using Marsad.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marsad.Controllers
{
    public class UpdateController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult GeoAreas(string type,int? IndicatorID)
        {
            type = SetType(type);
            ViewBag.GeoAreaID = new SelectList(db.GeoAreas.Where(x => x.Type.Equals(type)), "ID", "Name");
            int[] indicatorIds = db.Equations.Select(x => x.IndicatorID).Distinct().ToArray();
            ViewBag.Indicators = db.Indicators.Include(x=>x.Equations).Where(x => indicatorIds.Contains(x.ID)).ToList();
            if (IndicatorID.HasValue)
            {
                ViewBag.Indicator = db.Indicators.Include(x=>x.Equations).Where(x => x.ID == IndicatorID.Value).FirstOrDefault();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GeoAreas(int EquationID, int GeoAreaID)
        {
            var equation = db.Equations.Find(EquationID);
            var geoArea = db.GeoAreas.Find(GeoAreaID);
            if (equation == null || geoArea == null)
            {
                return HttpNotFound();
            }
            int[] years = equation.EquationYears.Select(x => x.Year).ToArray();
            Dictionary<int, float> elements = new Dictionary<int, float>();
            int eid;
            float val;
            foreach (string key in Request.Form.Keys)
            {
                if (key.StartsWith("elements_"))
                {
                    if (int.TryParse(key.Replace("elements_", ""), out eid) && float.TryParse(Request.Form[key], out val))
                    {
                        elements.Add(eid, val);
                    }
                }
            }

            var elementValues = db.ElementValues.Where(
                x => x.EquationID == equation.ID
                && elements.Keys.Contains(x.ElementID)
                && x.GeoAreaID == geoArea.ID
                && years.Contains(x.Year)).ToList();
            foreach (var elementValue in elementValues)
            {
                db.ElementValues.Remove(elementValue);
            }

            
            foreach(var elementID in elements.Keys)
            {
                foreach(int year in years)
                {
                    db.ElementValues.Add(new ElementValue()
                    {
                        ElementID = elementID,
                        EquationID = equation.ID,
                        GeoAreaID = geoArea.ID,
                        Value = elements[elementID],
                        Year=year
                    });
                }
                
            }
          
            db.SaveChanges();


            var type = geoArea.Type;
            type = SetType(type);

            ViewBag.GeoAreaID = new SelectList(db.GeoAreas.Where(x => x.Type.Equals(type)), "ID", "Name");
            ViewBag.EquationID = db.Equations.Include(x => x.EquationElements).Include(x => x.Indicator).ToList();
            return View();
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