using Marsad.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Marsad.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UpdateController : Controller
    {
        private ApplicationDbContext db;
        private UserStore<ApplicationUser> userStore;
        private ApplicationUserManager userManager;
        public UpdateController()
        {
            db = new ApplicationDbContext();
            userStore = new UserStore<ApplicationUser>(db);
            userManager = new ApplicationUserManager(userStore);
        }

        public ActionResult GeoAreas(string type)
        {
            var bundles = GetAllowedBundles();
            type = SetType(type);
            ViewBag.GeoAreaID = new SelectList(db.GeoAreas.Where(x => x.Type.Equals(type)), "ID", "Name");
            ViewBag.BundleID = new SelectList(bundles, "ID", "Name");
            var tempList = db.EquationElements.Select(x => new { x.Element.Name, x.Equation.IndicatorID }).ToList();
            Dictionary<string, List<int>> elements = new Dictionary<string, List<int>>();
            foreach (var v in tempList)
            {
                if (!elements.ContainsKey(v.Name))
                    elements.Add(v.Name, new List<int>());
                elements[v.Name].Add(v.IndicatorID);
            }
            ViewBag.Elements = elements;
            return View();
        }

        public ActionResult GetIndicatorEquations(int indicatorId, int geoAreaID)
        {
            var indicator = db.Indicators.Where(x => x.ID == indicatorId).Include(x => x.Equations).FirstOrDefault();
            if (indicator == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var equationIds = indicator.Equations.Select(x => x.ID).ToList();
            var equationYears = db.EquationYears.Where(x => equationIds.Contains(x.EquationID)).OrderBy(x => x.Year).ToList();
            var elementIDs = db.EquationElements.Where(x => equationIds.Contains(x.EquationID)).Select(x => x.ElementID).ToArray();
            var elementYearValues = db.ElementYearValues.Include(x => x.ApplicationUser).Where(x => x.GeoAreaID == geoAreaID && elementIDs.Contains(x.ElementID)).ToList();
            var calculatedValues = db.CalculatedValues.Where(x => x.GeoAreaID == geoAreaID && x.EquationYear.Equation.IndicatorID == indicatorId).ToDictionary(x => x.EquationYear.Year, x => x.Value);
            ViewBag.EquationYears = equationYears;
            ViewBag.GeoAreaID = geoAreaID;
            ViewBag.ElementYearValues = elementYearValues;
            ViewBag.CalculatedValues = calculatedValues;
            return View(indicator);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GeoAreas()
        {
            var geoAreaID = Request["GeoAreaID"];
            var equationYearID = Request["EquationYearID"];
            var action = Request["action"];
            string[] elementValuesKeys = Request.Form.AllKeys.Where(x => x.StartsWith("ElementValue_")).ToArray();
            GeoArea geoArea = null;
            EquationYear equationYear = null;
            if (!string.IsNullOrWhiteSpace(geoAreaID))
            {
                geoArea = db.GeoAreas.Find(int.Parse(geoAreaID));
            }
            if (!string.IsNullOrWhiteSpace(geoAreaID))
            {
                int _equationYearID = int.Parse(equationYearID);
                equationYear = db.EquationYears.Include(x => x.Equation.Indicator).Where(x => x.ID == _equationYearID).FirstOrDefault();
            }
            if (equationYear == null || geoArea == null)
            {
                return Json(new { success = false, error = "برجاء إدخال بيانات صحيحة" });
            }
            string userId = User.Identity.GetUserId();
            try
            {
                var calculate = false;
                if (action.Equals("delete"))
                {
                    DeleteUpdate(geoArea, equationYear);
                    DeleteCalculated(geoArea, equationYear);
                }
                else if (action.Equals("add"))
                {
                    calculate = AddUpdate(geoArea, equationYear, userId, elementValuesKeys);
                }
                else if (action.Equals("edit"))
                {
                    calculate = EditUpdate(geoArea, equationYear, userId, elementValuesKeys);
                }
                DeleteCalculated(geoArea, equationYear);
                if ((action.Equals("add") || action.Equals("edit")) && calculate)
                {
                    double value = CalculateEquation(equationYear, geoArea);
                    Commit(geoArea, equationYear, value);
                }

                db.SaveChanges();
                UpdateLog(geoArea, equationYear, action);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "برجاء إدخال بيانات صحيحة" });
            }
            return Json(new { success = true, action });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GeoAreasConfirm()
        {
            var geoAreaID = Request["GeoAreaID"];
            var equationYearID = Request["EquationYearID"];
            GeoArea geoArea = null;
            EquationYear equationYear = null;
            if (!string.IsNullOrWhiteSpace(geoAreaID))
            {
                geoArea = db.GeoAreas.Find(int.Parse(geoAreaID));
            }
            if (!string.IsNullOrWhiteSpace(geoAreaID))
            {
                int _equationYearID = int.Parse(equationYearID);
                equationYear = db.EquationYears.Include(x => x.Equation.Indicator).Where(x => x.ID == _equationYearID).FirstOrDefault();
            }
            if (equationYear == null || geoArea == null)
            {
                return Json(new { success = false, error = "برجاء إدخال بيانات صحيحة" });
            }
            double value = CalculateEquation(equationYear, geoArea);
            DeleteCalculated(geoArea, equationYear);
            Commit(geoArea, equationYear, value);
            db.SaveChanges();
            UpdateLog(geoArea, equationYear, "commit");
            return Json(new { success = true });
        }

        private void UpdateLog(GeoArea geoArea, EquationYear equationYear, string action)
        {
            string indicatorName = equationYear.Equation.Indicator.Name;
            string geoAreaName = geoArea.Name;
            string geoAreaType = geoArea.Type.Equals("Bundle") ? "نطاق جغرافي" : GeoArea.Types[geoArea.Type];
            string year = equationYear.Year.ToString();
            string values = "";
            int valuesCount = 0;
            foreach (ElementValue ev in equationYear.ElementValues.Where(x => x.GeoAreaID == geoArea.ID))
            {
                values += string.Format("{0} : {1}\r\n", ev.EquationElement.Element.Name, ev.Value);
                valuesCount += 1;
            }
            try
            {
                if (action == "delete")
                {
                    db.UpdateLogs.Add(new Models.UpdateLog()
                    {
                        ActionDate = DateTime.Now,
                        UserName = User.Identity.GetUserName() + " - " + User.GetName(),
                        Log = string.Format("تم حذف بيانات قيم المؤشر {0} لسنة {1} ل{2} {3}", indicatorName, year, geoAreaType, geoAreaName),
                        Details = values,
                        ApplicationUserId = User.Identity.GetUserId(),
                        ValuesCount = valuesCount,
                        Type = "delete"
                    });
                }
                else if (action == "add")
                {
                    db.UpdateLogs.Add(new Models.UpdateLog()
                    {
                        ActionDate = DateTime.Now,
                        UserName = User.Identity.GetUserName() + " - " + User.GetName(),
                        Log = string.Format("تم إضافة {4} بيانات قيم المؤشر {0} لسنة {1} ل{2} {3}", indicatorName, year, geoAreaType, geoAreaName, "وتأكيد"),
                        Details = values,
                        ApplicationUserId = User.Identity.GetUserId(),
                        ValuesCount = valuesCount,
                        Type = "add"
                    });
                }
                else if (action == "edit")
                {
                    db.UpdateLogs.Add(new Models.UpdateLog()
                    {
                        ActionDate = DateTime.Now,
                        UserName = User.Identity.GetUserName() + " - " + User.GetName(),
                        Log = string.Format("تم تحديث {4} بيانات قيم المؤشر {0} لسنة {1} ل{2} {3}", indicatorName, year, geoAreaType, geoAreaName, "وتأكيد"),
                        Details = values,
                        ApplicationUserId = User.Identity.GetUserId(),
                        ValuesCount = valuesCount,
                        Type = "edit"
                    });
                }

                else if (action == "commit")
                {
                    db.UpdateLogs.Add(new Models.UpdateLog()
                    {
                        ActionDate = DateTime.Now,
                        UserName = User.Identity.GetUserName() + " - " + User.GetName(),
                        Log = string.Format("تم تاكيد بيانات قيم المؤشر {0} لسنة {1} ل{2} {3}", indicatorName, year, geoAreaType, geoAreaName),
                        Details = values,
                        ApplicationUserId = User.Identity.GetUserId(),
                        ValuesCount = valuesCount,
                        Type = "commit"
                    });
                }

                db.SaveChanges();
            }
            catch
            {

            }
        }

        private void Commit(GeoArea geoArea, EquationYear equationYear, double value)
        {
            DateTime commitedAt = DateTime.Now;
            foreach (var elementValue in equationYear.ElementValues.Where(x => x.GeoAreaID == geoArea.ID))
            {
                elementValue.CommitedAt = commitedAt;
                elementValue.IsCommited = true;
            }
            db.CalculatedValues.Add(new CalculatedValue()
            {
                EquationYearID = equationYear.ID,
                GeoAreaID = geoArea.ID,
                Value = value
            });
        }

        private double CalculateEquation(EquationYear equationYear, GeoArea geoArea)
        {
            string equationText = equationYear.Equation.EquationText;
            foreach (var elementValue in equationYear.ElementValues.Where(x => x.GeoAreaID == geoArea.ID))
            {
                equationText = equationText.Replace("[" + elementValue.EquationElement.Element.Name + "]", "(CAST (" + elementValue.Value.ToString() + " as float(53)))");
            }
            var results = db.Database.SqlQuery<double>("SELECT CAST ((" + equationText + ") as float(53)) as V1").ToList();
            if (results.Count != 1)
            {
                throw new Exception("Error");
            }
            return (double)results[0];
        }

        private bool EditUpdate(GeoArea geoArea, EquationYear equationYear, string userId, string[] elementValuesKeys)
        {
            DeleteUpdate(geoArea, equationYear);
            return AddUpdate(geoArea, equationYear, userId, elementValuesKeys);
        }

        private bool AddUpdate(GeoArea geoArea, EquationYear equationYear, string userId, string[] elementValuesKeys)
        {
            var calculate = true;
            string values = "";
            var elementIds = new List<int>();
            foreach (string evs in elementValuesKeys)
            {
                string eeId = evs.Replace("ElementValue_", "");
                EquationElement equationElement = db.EquationElements.Find(int.Parse(eeId));
                if (equationElement == null)
                {
                    throw new Exception("Equation Element Not Found");
                }
                string strValue = Request[evs].Trim();
                double _fValue = 0;
                double? fValue = null;
                if (string.IsNullOrEmpty(strValue) || double.TryParse(strValue, out _fValue))
                {
                    if (!string.IsNullOrEmpty(strValue))
                        fValue = _fValue;
                    else
                        calculate = false;
                    values += string.Format("{0} : {1}\r\n", equationElement.Element.Name, strValue);
                    var createdAt = DateTime.Now;
                    var elementValue = new ElementValue()
                    {
                        EquationElementID = equationElement.ID,
                        EquationYearID = equationYear.ID,
                        GeoAreaID = geoArea.ID,
                        Value = fValue,
                        ApplicationUserID = userId,
                        CreatedAt = createdAt,
                    };
                    db.ElementValues.Add(elementValue);
                    elementIds.Add(equationElement.ElementID);
                }
                else
                {
                    throw new Exception("Value is in correct");
                }
            }
            if (calculate)
            {
                var elementYearValues = db.ElementYearValues.Where(x => x.GeoAreaID == geoArea.ID && x.Year == equationYear.Year && elementIds.Contains(x.ElementID)).ToList();
                foreach (var eyv in elementYearValues)
                {
                    eyv.IsCommited = true;
                    db.Entry(eyv).State = EntityState.Modified;
                }
            }
            return calculate;
        }

        private void DeleteUpdate(GeoArea geoArea, EquationYear equationYear)
        {
            var elementValues = db.ElementValues.Where(x => x.GeoAreaID == geoArea.ID && x.EquationYearID == equationYear.ID).ToList();
            var elementIds = elementValues.Select(x => x.EquationElement.ElementID).ToArray();
            for (int i = elementValues.Count - 1; i >= 0; i--)
            {
                db.ElementValues.Remove(elementValues[i]);
            }
            var elementYearValues = db.ElementYearValues.Where(x => x.GeoAreaID == geoArea.ID && x.Year == equationYear.Year && elementIds.Contains(x.ElementID)).ToList();
            foreach (var eyv in elementYearValues)
            {
                eyv.IsCommited = false;
                db.Entry(eyv).State = EntityState.Modified;
            }
        }

        private void DeleteCalculated(GeoArea geoArea, EquationYear equationYear)
        {
            var oldCalculatedValues = db.CalculatedValues.Where(x => x.EquationYearID == equationYear.ID && x.GeoAreaID == geoArea.ID).ToList();
            for (int i = oldCalculatedValues.Count - 1; i >= 0; i--)
            {
                db.CalculatedValues.Remove(oldCalculatedValues[i]);
            }
        }

        private IQueryable<Bundle> GetAllowedBundles()
        {
            var bundleIdsWithEquations = db.Bundles.Where(x => x.Indicators.Where(y => y.Equations.Any()).Any()).Select(x => x.ID).Distinct().ToArray();
            return db.Bundles.Where(x => bundleIdsWithEquations.Contains(x.ID));
        }

        private string SetType(string type)
        {
            if (type.Equals("Bundle"))
            {
                ViewBag.Type = type;
                ViewBag.TypeName = "نظاق جغرافي";
                return type;
            }
            if (string.IsNullOrWhiteSpace(type) || !GeoArea.Types.ContainsKey(type))
            {
                type = "Region";
            }
            ViewBag.Type = type;
            ViewBag.TypeName = GeoArea.Types[type];
            ViewBag.Parent = GeoArea.GetParentName(type);
            ViewBag.ParentName = GeoArea.Types[GeoArea.GetParentName(type)];
            return type;
        }
    }
}