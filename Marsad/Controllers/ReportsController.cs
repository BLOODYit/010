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
        public string[] GovNames
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["GovNames"].Split(','); }
        }

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
            foreach (var cv in calculatedVals)
            {
                if (!result.ContainsKey(cv.EquationYear.Equation.IndicatorID))
                {
                    IndicatorValues indicatorValues = new IndicatorValues()
                    {
                        BundleID = cv.EquationYear.Equation.Indicator.BundleID,
                        BundleName = cv.EquationYear.Equation.Indicator.Bundle.Name,
                        IndicatorID = cv.EquationYear.Equation.IndicatorID,
                        IndicatorName = cv.EquationYear.Equation.Indicator.Name,
                        Values = new Dictionary<int, double>()
                    };
                    result.Add(cv.EquationYear.Equation.IndicatorID, indicatorValues);
                }
                if (!result[cv.EquationYear.Equation.IndicatorID].Values.ContainsKey(cv.EquationYear.Year))
                {
                    result[cv.EquationYear.Equation.IndicatorID].Values.Add(cv.EquationYear.Year, cv.Value);
                }
            }
            ViewBag.Years = years.OrderBy(x => x);
            return View(result);
        }

        public ActionResult Bundles(int[] bundleIds)
        {
            ViewBag.Bundles = db.Bundles.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.bundleIds = bundleIds;

            var bundles = db.Bundles.AsQueryable();
            if (bundleIds != null && bundleIds.Length > 0)
            {
                bundles = bundles.Where(x => bundleIds.Contains(x.ID));
            }
            var result = bundles.Include(x => x.Indicators);
            return View(result.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult PendingLog(string sortOrder, string currentFilter, string searchString)
        {
            ViewBag.CurrentSort = sortOrder;
            if (searchString == null)
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var query = db.OfficerLogs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchString))
                query = query.Where(x => x.Name.Contains(searchString) || x.EntityName.Contains(searchString) || x.Notes.Contains(searchString));
            return View(query.ToList());
        }

        [AllowAnonymous]
        public ActionResult RegionReport(string backgroundColor, string startColor, string endColor, string items)
        {
            string[] govNames = GovNames;
            int[] govIds = db.GeoAreas.Where(x => govNames.Contains(x.Name) && x.Type.Equals("Governorate")).Select(x => x.ID).ToArray();
            var calculatedValues = db.CalculatedValues.Where(x => govIds.Contains(x.GeoAreaID)).Select(x => new { x.Value, x.GeoAreaID, x.GeoArea.Name, x.EquationYear.Year, x.EquationYear.Equation.IndicatorID }).ToArray();
            var indicatorIds = calculatedValues.Select(x => x.IndicatorID).Distinct().ToArray();
            var indicators = db.Indicators.Where(x => indicatorIds.Contains(x.ID));
            var bundleIds = indicators.Select(x => x.BundleID).Distinct().ToArray();
            var bundles = db.Bundles.Where(x => bundleIds.Contains(x.ID));
            ViewBag.BundleID = new SelectList(bundles, "ID", "Name");
            ViewBag.Indicators = indicators.Select(x => new { x.ID, x.Name, x.BundleID }).ToList();
            ViewBag.CalculatedValues = calculatedValues;
            if (!string.IsNullOrWhiteSpace(backgroundColor))
                ViewBag.backgroundColor = backgroundColor;
            if (!string.IsNullOrWhiteSpace(startColor))
                ViewBag.startColor = startColor;
            if (!string.IsNullOrWhiteSpace(endColor))
                ViewBag.endColor = endColor;
            if (string.IsNullOrEmpty(items))
                ViewBag.items = new string[] { "map", "table", "piechart", "barchart" };
            else
                ViewBag.items = items.ToLower().Split(',');
            return View();
        }

        [AllowAnonymous]
        public ActionResult CityBundles(string cityName)
        {
            var city = db.GeoAreas.Where(x => x.Name.Equals(cityName)).FirstOrDefault();
            if (city != null)
            {
                var bundlesIds = db.CalculatedValues.Where(x => x.GeoAreaID == city.ID)
                .Select(x => x.EquationYear.Equation.Indicator.Bundle.ID).ToArray();
                var bundles = db.Bundles.Where(x => bundlesIds.Contains(x.ID)).ToList();
                ViewBag.Bundles = bundles;
            }
            ViewBag.City = city;
            return View();
        }

        [AllowAnonymous]
        public ActionResult CityIndicators(int bundleId, int geoAreaID)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            var city = db.GeoAreas.Where(x => x.ID == geoAreaID).FirstOrDefault();
            if (city != null)
            {
                var _calculatedValues = db.CalculatedValues
                    .Where(x => x.GeoAreaID == city.ID && x.EquationYear.Equation.Indicator.BundleID == bundleId)
                    .Include(x => x.EquationYear.Equation.Indicator)
                    .Include(x => x.GeoArea)
                    .ToList();

                var calculatedValues = _calculatedValues.Select(x => new CalculatedValue()
                {
                    EquationYearID = x.EquationYearID,
                    GeoAreaID = x.GeoAreaID,
                    ID = x.ID,
                    Value = x.Value,
                    GeoArea = new GeoArea()
                    {
                        ID = x.GeoArea.ID,
                        Code = x.GeoArea.Code,
                        Name = x.GeoArea.Name
                    },
                    EquationYear = new EquationYear()
                    {
                        ID = x.EquationYear.ID,
                        Year = x.EquationYear.Year,
                        EquationID = x.EquationYear.EquationID,
                        Equation = new Equation()
                        {
                            ID = x.EquationYear.Equation.ID,
                            Indicator = new Indicator()
                            {
                                ID = x.EquationYear.Equation.Indicator.ID,
                                MeasureUnit = x.EquationYear.Equation.Indicator.MeasureUnit,
                                Name = x.EquationYear.Equation.Indicator.Name
                            }
                        }
                    }
                }).ToList();
                var years = db.CalculatedValues
                  .Where(x => x.GeoAreaID == city.ID && x.EquationYear.Equation.Indicator.BundleID == bundleId)
                  .Select(x => x.EquationYear.Year).Distinct().OrderBy(x => x).ToArray();

                ViewBag.CalculatedValues = calculatedValues;
                ViewBag.Years = years;
            }
            var bundle = db.Bundles.First(x => x.ID == bundleId);
            ViewBag.BundleID = bundleId;
            ViewBag.City = city;
            if (bundle != null)
                ViewBag.BundleName = bundle.Name;
            return View();
        }

        [AllowAnonymous]
        public ActionResult CityIndicator(int indicatorId)
        {
            var indicator = db.Indicators.Where(x => x.ID == indicatorId).FirstOrDefault();
            return View(indicator);
        }

        [AllowAnonymous]
        public ActionResult CityIndicatorTimeSeries(int indicatorId, int geoAreaId)
        {
            var indicator = db.Indicators.Where(x => x.ID == indicatorId).FirstOrDefault();
            var city = db.GeoAreas.Where(x => x.ID == geoAreaId).FirstOrDefault();
            var calculatedValues = db.CalculatedValues
                .Where(x => x.GeoAreaID == geoAreaId)
                .Where(x => x.EquationYear.Equation.Indicator.ID == indicatorId)
                .OrderBy(x => x.EquationYear.Year)
                .Select(x => new { x.EquationYear.Year, Value = Math.Round(x.Value, 2) })
                .ToList();
            ViewBag.Indicator = indicator;
            ViewBag.City = city;
            ViewBag.CalculatedValues = calculatedValues;
            return View();
        }

        [AllowAnonymous]
        public ActionResult CityIndicatorComparisonSelect(int indicatorId, int geoAreaId, int year)
        {
            var indicator = db.Indicators.Where(x => x.ID == indicatorId).FirstOrDefault();
            var city = db.GeoAreas.Where(x => x.ID == geoAreaId).FirstOrDefault();
            ViewBag.Indicator = indicator;
            ViewBag.City = city;
            ViewBag.Year = year;
            return View();
        }

        [AllowAnonymous]
        public ActionResult CityIndicatorComparison(int indicatorId, int geoAreaId, int year, string[] included)
        {
            var indicator = db.Indicators.Where(x => x.ID == indicatorId).FirstOrDefault();
            var city = db.GeoAreas.Where(x => x.ID == geoAreaId).FirstOrDefault();
            var calculatedValues = db.CalculatedValues
                .Where(x => x.GeoAreaID == geoAreaId)
                .Where(x => x.EquationYear.Equation.Indicator.ID == indicatorId)
                .Where(x => x.EquationYear.Year == year)
                .OrderBy(x => x.EquationYear.Year)
                .Select(x => new { Value=Math.Round(x.Value,2) })
                .ToList();

            double? intLow = null;
            double? intHigh = null;
            double? locLow = null;
            double? locHigh = null;
            if (included.Contains("intLow"))
                intLow = 0;
            if (included.Contains("intHigh"))
                intHigh = 0;
            if (included.Contains("locLow"))
                locLow = 0;
            if (included.Contains("locHigh"))
                locHigh = 0;
            var limit = db.IndicatorLimits.Where(x => x.IndicatorID == indicatorId && x.Year <= year).OrderByDescending(x => x.Year).FirstOrDefault();
            if (limit != null)
            {
                if (included.Contains("intLow") && limit.IntLow.HasValue)
                    intLow = Math.Round(limit.IntLow.Value,2);
                if (included.Contains("intHigh") && limit.IntHigh.HasValue)
                    intHigh = Math.Round(limit.IntHigh.Value, 2);
                if (included.Contains("locLow") && limit.LocLow.HasValue)
                    locLow = Math.Round(limit.LocLow.Value, 2);
                if (included.Contains("locHigh") && limit.LocHigh.HasValue)
                    locHigh = Math.Round(limit.LocHigh.Value, 2);
            }
            ViewBag.Indicator = indicator;
            ViewBag.City = city;
            ViewBag.Year = year;
            ViewBag.CalculatedValues = calculatedValues;
            ViewBag.IntLow = intLow;
            ViewBag.IntHigh = intHigh;
            ViewBag.LocLow = locLow;
            ViewBag.LocHigh = locHigh;
            return View();
        }

        [AllowAnonymous]
        public ActionResult CityYearRange(int bundleId, string cityName)
        {
            var city = db.GeoAreas.Where(x => x.Name.Equals(cityName)).FirstOrDefault();
            if (city != null)
            {
                var years = db.CalculatedValues
                    .Where(x => x.GeoAreaID == city.ID)
                    .Where(x => x.EquationYear.Equation.Indicator.BundleID == bundleId)
                    .Select(x => x.EquationYear.Year).OrderBy(x => x).Distinct().ToList();
                var bundle = db.Bundles.First(x => x.ID == bundleId);
                ViewBag.Years = years;
                ViewBag.CityName = cityName;
                ViewBag.BundleId = bundleId;
                if (bundle != null)
                    ViewBag.BundleName = bundle.Name;
            }
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