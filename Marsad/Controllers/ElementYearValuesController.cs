using Marsad.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marsad.Controllers
{
    public class ElementYearValuesController : BaseController
    {
        UserStore<ApplicationUser> userStore;
        UserManager<ApplicationUser> userManager;
        public ElementYearValuesController()
        {
            db = new ApplicationDbContext();
            userStore = new UserStore<ApplicationUser>(db);
            userManager = new UserManager<ApplicationUser>(userStore);
        }
        // GET: ElementYearValues        
        public ActionResult Index(string type = "Region")
        {
            type = SetType(type);
            ViewBag.GeoAreaID = new SelectList(db.GeoAreas.Where(x => x.Type.Equals(type)), "ID", "Name");
            return View();
        }

        public ActionResult GetElementValues(int year,int geoArea)
        {
            var elements = db.Elements.AsQueryable();
            var userId = User.Identity.GetUserId();
            var isDeleted = userManager.Users.Where(x => x.Id == userId).FirstOrDefault().IsDeleted;
            if (isDeleted)
                throw new Exception("Not Allowed");
            if (User.IsInRole("Officer"))
            {
                
                elements = elements.Where(x => x.ApplicationUsers.Where(y => y.Id == userId).Any());
            }
            var elementIds = elements.Select(x => x.ID).ToArray();
            var dataSourceIds = elements.Select(x => x.DataSourceID).Distinct().ToArray();
            ViewBag.ElementYearValues = db.ElementYearValues.Where(x => elementIds.Contains(x.ElementID) && x.Year == year &&x.GeoAreaID==geoArea).ToList();
            ViewBag.Elements = elements.ToList();
            ViewBag.DataSources = db.DataSources.Where(x => dataSourceIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.Name);            
            return View();
        }

        public JsonResult UpdateElementValues(int year,int geoArea, int[] elementIds, double[] values)
        {

            if (elementIds == null || values == null || elementIds.Length != values.Length)
                throw new Exception("Invalid Inputs");
            var userElementIds = elementIds;
            var userId = User.Identity.GetUserId();
            var isDeleted = userManager.Users.Where(x => x.Id == userId).FirstOrDefault().IsDeleted;
            if (isDeleted)
                throw new Exception("Not Allowed");
            if (User.IsInRole("Officer"))
            {
                userElementIds = db.Elements.Where(x => x.ApplicationUsers.Where(y => y.Id == userId).Any()).Select(x => x.ID).ToArray();
            }
            var oldElementValues = db.ElementYearValues.Where(x => x.Year == year && userElementIds.Contains(x.ElementID)).ToList();
            db.ElementYearValues.RemoveRange(oldElementValues);
            var createdAt = DateTime.Now;
            Dictionary<int, double> elemets = new Dictionary<int, double>();
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
            var notes = string.Format("نظاق جغرافي ({0}) لسنة {1}, القيم (", geoAreaName,year);
            var valueCounts = 0;
            foreach(var id in elemets.Keys)
            {
                if (elementNames.ContainsKey(id))
                {
                    notes += string.Format("{0}:{1}, ", elementNames[id], elemets[id]);
                    valueCounts++;
                }
            }
            notes = notes.TrimEnd(' ').TrimEnd(',');
            notes += ")";
            Log(LogAction.Create, new ElementYearValue(), notes);
            if (User.IsInRole("Officer"))
            {                
                var user = userManager.Users.Include(x => x.Entity).Where(x => x.Id == userId).FirstOrDefault();
                if (user != null)
                {                  
                    db.OfficerLogs.Add(new OfficerLog()
                    {
                        ApplicationUserId = user.Id,
                        Name = user.Name,
                        UserName = user.UserName,
                        EntityName = user.Entity != null ? user.Entity.Name : user.Name,
                        ActionDate = DateTime.Now,
                        ValuesCount = valueCounts,
                        Notes = notes,
                        Year = year
                    }) ;
                    db.SaveChanges();
                }
                
            }                     
            return Json(new { success=true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteElementValues(int year, int geoArea, int[] elementIds)
        {

            if (elementIds == null)
                throw new Exception("Invalid Inputs");
            var userElementIds = elementIds;
            var userId = User.Identity.GetUserId();
            var isDeleted = userManager.Users.Where(x => x.Id == userId).FirstOrDefault().IsDeleted;
            if (isDeleted)
                throw new Exception("Not Allowed");
            if (User.IsInRole("Officer"))
            {
                userElementIds = db.Elements.Where(x => x.ApplicationUsers.Where(y => y.Id == userId).Any()).Select(x => x.ID).ToArray();
            }
            var oldElementValues = db.ElementYearValues.Where(x => x.Year == year && userElementIds.Contains(x.ElementID)).ToList();
            db.ElementYearValues.RemoveRange(oldElementValues);
            var createdAt = DateTime.Now;
            List<int> elemets = new List<int>();
            for (int i = 0; i < elementIds.Length; i++)
            {
                if (!userElementIds.Contains(elementIds[i]))
                    continue;                
                    elemets.Add(elementIds[i]);                
            }
            db.SaveChanges();
            elementIds = elemets.ToArray();
            var elementNames = db.Elements.Where(x => elementIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.Name);
            var geoAreaName = "";
            var gA = db.GeoAreas.Find(geoArea);
            if (gA != null)
                geoAreaName = gA.Name;
            var notes = string.Format("نظاق جغرافي ({0}) لسنة {1}, حذف القيم (", geoAreaName, year);
            var valueCounts = 0;
            foreach (var id in elemets)
            {
                if (elementNames.ContainsKey(id))
                {
                    notes += string.Format("{0}, ", elementNames[id]);
                    valueCounts++;
                }
            }
            notes = notes.TrimEnd(' ').TrimEnd(',');
            notes += ")";
            Log(LogAction.Delete, new ElementYearValue(), notes);
            if (User.IsInRole("Officer"))
            {
                var user = userManager.Users.Include(x => x.Entity).Where(x => x.Id == userId).FirstOrDefault();
                if (user != null)
                {
                    db.OfficerLogs.Add(new OfficerLog()
                    {
                        ApplicationUserId = user.Id,
                        Name = user.Name,
                        UserName = user.UserName,
                        EntityName = user.Entity != null ? user.Entity.Name : user.Name,
                        ActionDate = DateTime.Now,
                        ValuesCount = valueCounts,
                        Notes = notes,
                        Year = year
                    });
                    db.SaveChanges();
                }
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
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