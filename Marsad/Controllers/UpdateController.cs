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
        public ActionResult GeoAreas(string type)
        {
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