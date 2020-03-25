using Marsad.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Marsad.Models.ViewModels;

namespace Marsad.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        #region ReportActions
        public ActionResult Period(string type = "Region")
        {
            type = SetType(type);
            ViewBag.GeoAreaID = new SelectList(db.GeoAreas.Where(x => x.Type.Equals(type)), "ID", "Name");
            var indicatorIds = db.Indicators.Where(x => x.Equations.Any()).Select(x => x.BundleID).Distinct().ToArray();
            var bundles = db.Bundles.Where(x => indicatorIds.Contains(x.ID));
            ViewBag.BundleID = new SelectList(bundles, "ID", "Name");
            return View();
        }

        public ActionResult GeoArea()
        {
            var indicatorIds = db.Indicators.Where(x => x.Equations.Any()).Select(x => x.BundleID).Distinct().ToArray();
            var bundles = db.Bundles.Where(x => indicatorIds.Contains(x.ID));
            ViewBag.BundleID = new SelectList(bundles, "ID", "Name");
            return View();
        }

        public ActionResult IndicatorCategories(string type = "Region")
        {
            type = SetType(type);
            ViewBag.GeoAreaID = new SelectList(db.GeoAreas.Where(x => x.Type.Equals(type)), "ID", "Name");
            return View();
        }

        public ActionResult GetYears(int geoAreaId)
        {
            int[] years = db.CalculatedValues.Where(x => x.GeoAreaID == geoAreaId).Select(x => x.EquationYear.Year).Distinct().ToArray();            
            return View(years);
        }

        public ActionResult GetIndicatorCategories(int geoAreaId, int[] years)
        {
            var calculatedVals = db.CalculatedValues.Where(x => years.Contains(x.EquationYear.Year) && x.GeoAreaID == geoAreaId)
                .Include(x => x.EquationYear.Equation.Indicator)
                .Include(x => x.EquationYear.Equation.Indicator.Bundle)
                .ToList();
            Dictionary<int, IndicatorValues> result = new Dictionary<int, IndicatorValues>();
            foreach(var cv in calculatedVals)
            {
                if (!result.ContainsKey(cv.EquationYear.Equation.IndicatorID))
                {
                    IndicatorValues indicatorValues = new IndicatorValues()
                    {
                        BundleID = cv.EquationYear.Equation.Indicator.BundleID,
                        BundleName = cv.EquationYear.Equation.Indicator.Bundle.Name,
                        IndicatorID = cv.EquationYear.Equation.IndicatorID,
                        IndicatorName = cv.EquationYear.Equation.Indicator.Name,
                        Values = new Dictionary<int, float>()
                    };
                    result.Add(cv.EquationYear.Equation.IndicatorID, indicatorValues);
                }
                if (!result[cv.EquationYear.Equation.IndicatorID].Values.ContainsKey(cv.EquationYear.Year))
                {
                    result[cv.EquationYear.Equation.IndicatorID].Values.Add(cv.EquationYear.Year,cv.Value);
                }
            }
            ViewBag.Years = years.OrderBy(x => x);
            return View(result);
        }

        public ActionResult Bundles()
        {
            var bundles = db.Bundles.Include(x => x.Indicators).ToList();
            return View(bundles);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult PendingLog(string sortOrder, string currentFilter, string searchString)
        {
            ViewBag.CurrentSort = sortOrder;
            if (searchString == null)
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var query = db.ElementYearValues.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchString))
                query = query.Where(x => x.Element.Name.Contains(searchString));
            var result = query.Include(x => x.ApplicationUser)
                .Include(x => x.Element)
                .Where(x => x.IsCommited == false)
                .GroupBy(x => new { x.CreatedAt, x.ApplicationUser, x.Year, x.GeoArea })
                .Select(x => new PendingLog()
                {
                    ActionDate = x.Key.CreatedAt,
                    UserName = x.Key.ApplicationUser.Name,
                    Entity = x.Key.ApplicationUser.Entity,
                    Log = "تحديث قيم نطاق (" + x.Key.GeoArea.Name + ")",
                    Year = x.Key.Year,
                    ApplicationUserID = x.Key.ApplicationUser.Id,
                    GeoAreaID = x.Key.GeoArea.ID
                })
                .OrderBy(x => x.ActionDate)
                .ToList();
            var details = db.ElementYearValues.Include(x => x.Element).Where(x => x.IsCommited == false);
            if (!string.IsNullOrWhiteSpace(searchString))
                details = details.Where(x => x.Element.Name.Contains(searchString));
            ViewBag.Details = details;
            return View(result);
        }

        [AllowAnonymous]
        public ActionResult RegionReport()
        {
            var indicators = db.Indicators.Where(x => x.Equations.Any());
            var bundleIds = indicators.Select(x => x.BundleID).Distinct().ToArray();
            var bundles = db.Bundles.Where(x => bundleIds.Contains(x.ID));
            ViewBag.BundleID = new SelectList(bundles, "ID", "Name");
            ViewBag.Indicators = indicators.Select(x => new { x.ID, x.Name, x.BundleID }).ToList();
            ViewBag.CalculatedValues = db.CalculatedValues.Select(x => new { x.Value, x.GeoAreaID, x.EquationYear.Year, x.EquationYear.Equation.IndicatorID });
            return View();
        }
        #endregion

        #region AjaxActions
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

        public ActionResult GetIndicatorYears(int indicatorId)
        {
            var indicator = db.Indicators.Where(x => x.ID == indicatorId).Include(x => x.Equations).FirstOrDefault();
            var equationIds = indicator.Equations.Select(x => x.ID).ToList();
            var equationYears = db.EquationYears.Where(x => equationIds.Contains(x.EquationID)).OrderBy(x => x.Year).ToList();

            if (indicator == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.EquationYears = equationYears;
            var equationYearsId = equationYears.Select(x => x.ID).ToArray();
            var geoAreaIds = db.ElementValues.Where(x => equationYearsId.Contains(x.EquationYearID)).Select(x => x.GeoAreaID).Distinct().ToArray();
            var geoAreas = db.GeoAreas.Where(x => geoAreaIds.Contains(x.ID)).ToList();
            ViewBag.GovAreas = geoAreas;
            return View(indicator);
        }

        public ActionResult GetPeriod(int indicatorId, int geoAreaID, int[] year)
        {
            List<CalculatedValue> results = GetData(indicatorId, new int[] { geoAreaID }, year);
            return View(results);
        }

        public ActionResult GetGeo(int indicatorId, int[] geoAreaIds, int year)
        {
            List<CalculatedValue> results = GetData(indicatorId, geoAreaIds, new int[] { year });
            return View(results);
        }
        #endregion

        private List<CalculatedValue> GetData(int indicatorId, int[] geoAreaIds, int[] years)
        {
            List<CalculatedValue> result = new List<CalculatedValue>();
            Indicator indicator = db.Indicators.Where(x => x.ID == indicatorId).FirstOrDefault();
            if (indicator == null)
                throw new Exception("Indicator Not Found");
            foreach (int geoAreaId in geoAreaIds)
            {
                GeoArea geoArea = db.GeoAreas.Where(x => x.ID == geoAreaId).FirstOrDefault();
                if (geoArea == null)
                    throw new Exception("GeoArea Not Found");
                foreach (int year in years)
                {
                    var calcVal = db.CalculatedValues
                            .Where(x => x.GeoAreaID == geoAreaId && x.EquationYear.Equation.IndicatorID == indicatorId && x.EquationYear.Year == year)
                            .Include(x => x.GeoArea)
                            .Include(x => x.EquationYear.Equation.Indicator)
                            .FirstOrDefault();
                    if (calcVal != null)
                        result.Add(calcVal);
                }
            }
            return result;
        }

        private string SetType(string type)
        {
            if (string.IsNullOrWhiteSpace(type) || !Marsad.Models.GeoArea.Types.ContainsKey(type))
            {
                type = "Region";
            }
            ViewBag.Type = type;
            ViewBag.TypeName = Marsad.Models.GeoArea.Types[type];
            ViewBag.Parent = Marsad.Models.GeoArea.GetParentName(type);
            ViewBag.ParentName = Marsad.Models.GeoArea.Types[Marsad.Models.GeoArea.GetParentName(type)];
            return type;
        }

        private string GetElementValue(EquationElement ee, int geoAreaID, int year)
        {
            var value = ee.ElementValues.Where(x => x.GeoAreaID == geoAreaID && x.EquationYear.Year == year).FirstOrDefault();
            if (value == null)
            {
                throw new Exception(string.Format("لا يوجد قيمة للمؤشر {0} في سنة {1}", ee.Element.Name, year));
            }
            return value.Value.ToString();
        }



    }
}