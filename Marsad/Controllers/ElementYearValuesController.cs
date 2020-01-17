using Marsad.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marsad.Controllers
{
    public class ElementYearValuesController : BaseController
    {
        // GET: ElementYearValues
        ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(string type = "Region")
        {
            type = SetType(type);
            ViewBag.GeoAreaID = new SelectList(db.GeoAreas.Where(x => x.Type.Equals(type)), "ID", "Name");
            return View();
        }

        public ActionResult GetElementValues(int year,int geoArea)
        {
            var elements = db.Elements.AsQueryable();
            if (User.IsInRole("Officer"))
            {
                var userId = User.Identity.GetUserId();
                elements = elements.Where(x => x.ApplicationUsers.Where(y => y.Id == userId).Any());
            }
            var elementIds = elements.Select(x => x.ID).ToArray();
            var dataSourceIds = elements.Select(x => x.DataSourceID).Distinct().ToArray();
            ViewBag.ElementYearValues = db.ElementYearValues.Where(x => elementIds.Contains(x.ElementID) && x.Year == year &&x.GeoAreaID==geoArea).ToList();
            ViewBag.Elements = elements.ToList();
            ViewBag.DataSources = db.DataSources.Where(x => dataSourceIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.Name);            
            return View();
        }

        public JsonResult UpdateElementValues(int year,int geoArea, int[] elementIds, float[] values)
        {

            if (elementIds == null || values == null || elementIds.Length != values.Length)
                throw new Exception("Invalid Inputs");
            var userElementIds = elementIds;
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Officer"))
            {
                userElementIds = db.Elements.Where(x => x.ApplicationUsers.Where(y => y.Id == userId).Any()).Select(x => x.ID).ToArray();
            }
            var oldElementValues = db.ElementYearValues.Where(x => x.Year == year && userElementIds.Contains(x.ElementID)).ToList();
            db.ElementYearValues.RemoveRange(oldElementValues);
            var createdAt = DateTime.Now;
            Dictionary<int, float> elemets = new Dictionary<int, float>();
            for (int i = 0; i < elementIds.Length; i++)
            {
                if (!userElementIds.Contains(elementIds[i]))
                    continue;
                db.ElementYearValues.Add(new ElementYearValue()
                {
                    ApplicationUserID = userId,
                    CreatedAt = createdAt,
                    ElementID = elementIds[i],
                    Value = values[i],
                    Year = year,
                    GeoAreaID=geoArea,
                    IsCommited=false
                });
                if (!elemets.ContainsKey(elementIds[i]))
                {
                    elemets.Add(elementIds[i], values[i]);
                }                
            }
            db.SaveChanges();
            elementIds = elemets.Keys.ToArray();
            var elementNames = db.Elements.Where(x => elementIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.Name);
            var geoAreaName = "";
            var gA = db.GeoAreas.Find(geoArea);
            if (gA != null)
                geoAreaName = gA.Name;
            var notes = "";
            foreach(var id in elemets.Keys)
            {
                if (elementNames.ContainsKey(id))
                {
                    notes += string.Format("نطاق جغرافي {2}, قيم {0} : {1}", elementNames[id], elemets[id],geoAreaName);
                }
            }
            Log(LogAction.Create, new ElementYearValue(), notes);
            return Json(new { success=true }, JsonRequestBehavior.AllowGet);
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

    }
}