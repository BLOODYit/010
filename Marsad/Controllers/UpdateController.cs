using Marsad.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Marsad.Controllers
{
    [Authorize(Roles ="Admin,Officer")]
    public class UpdateController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult GeoAreas(string type)
        {
            type = SetType(type);
            ViewBag.GeoAreaID = new SelectList(db.GeoAreas.Where(x => x.Type.Equals(type)), "ID", "Name");
            var indicatorIds = db.Indicators.Where(x => x.Equations.Any()).Select(x => x.BundleID).Distinct().ToArray();
            var bundles = db.Bundles.Where(x => indicatorIds.Contains(x.ID));
            ViewBag.BundleID = new SelectList(bundles, "ID", "Name");
            return View();
        }

        public ActionResult GetIndicatorEquations(int indicatorId, int geoAreaID)
        {
            var indicator = db.Indicators.Where(x => x.ID == indicatorId).Include(x => x.Equations).FirstOrDefault();
            var equationIds = indicator.Equations.Select(x => x.ID).ToList();
            var equationYears = db.EquationYears.Where(x => equationIds.Contains(x.EquationID)).OrderBy(x => x.Year).ToList();

            if (indicator == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.EquationYears = equationYears;
            ViewBag.GeoAreaID = geoAreaID;
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
                equationYear = db.EquationYears.Find(int.Parse(equationYearID));
            }
            if (equationYear == null || geoArea == null)
            {
                return Json(new { success = false, error = "برجاء إدخال بيانات صحيحة" });
            }
            List<ElementValue> elementValues = db.ElementValues.Where(x => x.GeoAreaID == geoArea.ID && x.EquationYearID == equationYear.ID).ToList();
            foreach(ElementValue ev in elementValues)
            {
                db.Entry(ev).State = EntityState.Deleted;
            }
            if (action == "add" || action == "edit")
            {
                foreach (string evs in elementValuesKeys)
                {
                    string eeId = evs.Replace("ElementValue_", "");
                    EquationElement equationElement = db.EquationElements.Find(int.Parse(eeId));
                    if (equationElement == null)
                    {
                        return Json(new { success = false, error = "برجاء إدخال بيانات صحيحة" });
                    }
                    string strValue = Request[evs];
                    if (float.TryParse(strValue, out float fValue))
                    {
                        db.ElementValues.Add(new ElementValue()
                        {
                            EquationElementID= equationElement.ID,
                            EquationYearID=equationYear.ID,
                            GeoAreaID=geoArea.ID,
                            Value=fValue
                        });
                    }
                    else
                    {
                        return Json(new { success = false, error = "برجاء إدخال بيانات صحيحة" });
                    }
                }
            }
            db.SaveChanges();
            return Json(new { success = true, action });
        }

        public ActionResult GeoAreaBundles()
        {
            return View();
        }
        // GET: Update
        public ActionResult Index()
        {
            return View();
        }

        private string SetType(string type)
        {
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