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
    [Authorize]
    public class ReportsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

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

        public ActionResult GetPeriod(int indicatorId, int geoAreaID, int year1, int year2)
        {
            var indicator = db.Indicators.Find(indicatorId);
            if (indicator == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.IndicatorName = indicator.Name;
            ViewBag.Year1 = year1;
            ViewBag.Year2 = year2;
            var equation1 = db.Equations.Where(x => x.IndicatorID == indicatorId && x.EquationYears.Where(y => y.Year == year1).Any()).FirstOrDefault();
            var equation2 = db.Equations.Where(x => x.IndicatorID == indicatorId && x.EquationYears.Where(y => y.Year == year2).Any()).FirstOrDefault();
            if (equation1 == null)
            {
                ViewBag.Error = "لا يوجد معادلة للسنة الاولى لهذا المؤشر";
                return View();
            }
            if (equation2 == null)
            {
                ViewBag.Error = "لا يوجد معادلة للسنة الثانية لهذا المؤشر";
                return View();
            }
            bool success = true;
            string error = "";
            ViewBag.Error = "";
            foreach (var ee in equation1.EquationElements)
            {
                try
                {
                    equation1.EquationText = equation1.EquationText.Replace("[" + ee.Element.Name + "]", GetElementValue(ee, geoAreaID, year1));
                }
                catch (Exception ex)
                {
                    error += ex.Message + " - ";
                    success = false;
                }
            }

            foreach (var ee in equation2.EquationElements)
            {
                try
                {
                    equation2.EquationText = equation2.EquationText.Replace("[" + ee.Element.Name + "]", GetElementValue(ee, geoAreaID, year2));
                }
                catch (Exception ex)
                {
                    error += ex.Message + " - ";
                    success = false;
                }
            }
            if (!success)
            {
                ViewBag.Error = error;
                return View();
            }
            List<PeriodReport> result = new List<PeriodReport>();
            try
            {
                result = db.Database.SqlQuery<PeriodReport>("SELECT CAST ((" + equation1.EquationText + ") as float) as V1,CAST ((" + equation2.EquationText + ") as float) as V2").ToList();
            }
            catch (Exception ex)
            {

            }
            if (result.Count == 0)
            {
                ViewBag.Error = "خطأ في تنفيذ العملية";
                return View();
            }
            return View(result[0]);
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

        public ActionResult GetGeo(int indicatorId, int[] geoAreaIds, int year)
        {
            var indicator = db.Indicators.Find(indicatorId);
            if (indicator == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.IndicatorName = indicator.Name;
            ViewBag.Year = year;
            ViewBag.GeoAreaIds = geoAreaIds;
            List<Equation> equations = new List<Equation>();
            var equation = db.Equations.Where(x => x.IndicatorID == indicatorId && x.EquationYears.Where(y => y.Year == year).Any()).FirstOrDefault();
            if (equation == null)
            {
                ViewBag.Error = "لا يوجد معادلة للمؤشر في هذه السنة";
                return View();
            }
            ViewBag.Error = "";
            List<GeoReport> result = new List<GeoReport>();
            Dictionary<int, string> GeoAreas = db.GeoAreas.Where(x => geoAreaIds.Contains(x.ID)).ToDictionary(x=>x.ID,x=>x.Name);

            foreach (int geoAreaID in geoAreaIds)
            {
                bool success = true;
                string error = "";
                var equationText = equation.EquationText;
                foreach (var ee in equation.EquationElements)
                {
                    try
                    {
                        equationText = equationText.Replace("[" + ee.Element.Name + "]", GetElementValue(ee, geoAreaID, year));
                    }
                    catch (Exception ex)
                    {
                        error += ex.Message + " - ";
                        success = false;
                    }
                }
                if (!success)
                {
                    ViewBag.Error += error;
                }
                else
                {
                    List<double> vals = db.Database.SqlQuery<double>("SELECT CAST ((" + equationText + ") as float) as V").ToList();
                    if (vals.Count > 0)
                    {
                        result.Add(new GeoReport() { 
                            GeoAreaID=geoAreaID,
                            V=vals[0],
                            GeoAreaName = GeoAreas[geoAreaID]
                        });
                    }
                }
            }
            if (result.Count == 0)
            {
                ViewBag.Error = "خطأ في تنفيذ العملية";
                return View();
            }
            return View(result);
        }
    }
}