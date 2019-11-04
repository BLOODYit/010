using Marsad.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marsad.Controllers
{
    public class ReportsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Period()
        {
            var equationIds = db.ElementValues.Select(x => x.EquationID).Distinct().ToArray();
            var indicatorIds = db.Equations.Where(x => equationIds.Contains(x.ID)).Select(x => x.IndicatorID).Distinct().ToArray();
            ViewBag.Indicators = db.Indicators.Where(x => indicatorIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.Name);
            return View();
        }

        public ActionResult GeoArea()
        {

            var equationIds = db.ElementValues.Select(x => x.EquationID).Distinct().ToArray();
            var indicatorIds = db.Equations.Where(x => equationIds.Contains(x.ID)).Select(x => x.IndicatorID).Distinct().ToArray();
            ViewBag.Indicators = db.Indicators.Where(x => indicatorIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.Name);
            return View();
        }

        [HttpPost]
        public ActionResult Period(int period1,int period2,int indicatorID)
        {
            var equationIds = db.ElementValues.Select(x => x.EquationID).Distinct().ToArray();
            var indicatorIds = db.Equations.Where(x => equationIds.Contains(x.ID)).Select(x => x.IndicatorID).Distinct().ToArray();
            ViewBag.Indicators = db.Indicators.Where(x => indicatorIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.Name);
            ViewBag.Result = "برجاء كتابة معادلة صحيحة";
            return View();
        }
        [HttpPost]
        public ActionResult GeoArea(int geoArea1,int geoArea2,int indicatorID)
        {

            var equationIds = db.ElementValues.Select(x => x.EquationID).Distinct().ToArray();
            var indicatorIds = db.Equations.Where(x => equationIds.Contains(x.ID)).Select(x => x.IndicatorID).Distinct().ToArray();
            ViewBag.Indicators = db.Indicators.Where(x => indicatorIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.Name);
            ViewBag.Result = "برجاء كتابة معادلة صحيحة";
            return View();
        }

        public JsonResult GetGeoAreas(int indicatorID)
        {
            var equationIds = db.Equations.Where(x=>x.IndicatorID==indicatorID).Select(x => x.ID).ToArray();
            var geoAreaIds = db.ElementValues.Where(x => equationIds.Contains(x.EquationID)).Select(x => x.GeoAreaID).Distinct().ToArray();
            var geoAreas = db.GeoAreas.Where(x => geoAreaIds.Contains(x.ID)).ToArray();
            return Json(geoAreas,JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult GetYears(int indicatorID)
        {
            var equationIds = db.Equations.Where(x => x.IndicatorID == indicatorID).Select(x => x.ID).ToArray();
            var years = db.ElementValues.Where(x => equationIds.Contains(x.EquationID)).Select(x => x.Year).Distinct().ToArray();            
            return Json(years, JsonRequestBehavior.AllowGet);
        }
    }
}