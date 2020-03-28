using Marsad.Models;
using Marsad.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;

namespace Marsad.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        ApplicationDbContext db;
        UserStore<ApplicationUser> userStore;
        ApplicationUserManager userManager;
        RoleStore<IdentityRole> roleStore;
        RoleManager<IdentityRole> roleManager;
        public HomeController()
        {
            db = new ApplicationDbContext();
            userStore = new UserStore<ApplicationUser>(db);
            userManager = new ApplicationUserManager(userStore);
            roleStore = new RoleStore<IdentityRole>(db);
            roleManager = new RoleManager<IdentityRole>(roleStore);
        }

        public ActionResult Index()
        {
            ViewBag.IndicatorCount = db.Indicators.Count();
            ViewBag.EntitiesCount = db.Entities.Count();
            var officerCount = 0;
            var role = roleManager.FindByName("Officer");
            if (role != null)
                officerCount = role.Users.Count();
            ViewBag.OfficersCount = officerCount;
            ViewBag.ParentIndicatorCount = db.Indicators.Where(x => !x.HasParent).Count();
            ViewBag.ChildIndicatorCount = db.Indicators.Where(x => x.HasParent).Count();
            ViewBag.BundleCount = db.Bundles.Count();
            ViewBag.DataSourceCount = db.DataSources.Count();
            ViewBag.DataSourceGroupCount = db.DataSourceGroups.Count();
            ViewBag.GeoAreas = db.GeoAreas.GroupBy(x => x.Type).ToDictionary(g => g.Key, g => g.Count());
            var officerIds = new List<string>();
            if (role != null)
            {
                officerIds = role.Users.Select(x => x.UserId).ToList();
            }

            var q = (from t in db.OfficerLogs
                     where officerIds.Contains(t.ApplicationUserId)
                     group t by new { t.ApplicationUserId }
                    into g
                     select new { ID = (from t2 in g select t2.ID).Max() }).Select(x => x.ID).ToArray();

            var lastUpdates = db.OfficerLogs.Where(x => q.Contains(x.ID)).ToList();
            var totalUpdates = db.OfficerLogs
                .Where(x => officerIds.Contains(x.ApplicationUserId)).GroupBy(x => new { x.ApplicationUserId }).Select(x => new
                {
                    ApplicationUserId = x.Key.ApplicationUserId,
                    Sum = x.Sum(y => y.ValuesCount)
                }).ToList();
            Dictionary<string, OfficerKPI> officerKPIs = new Dictionary<string, OfficerKPI>();
            foreach (OfficerLog log in lastUpdates)
            {
                if (!officerKPIs.ContainsKey(log.ApplicationUserId))
                    officerKPIs.Add(log.ApplicationUserId, new OfficerKPI());
                officerKPIs[log.ApplicationUserId].ApplicationUserId = log.ApplicationUserId;
                officerKPIs[log.ApplicationUserId].EntityName = log.EntityName;

                officerKPIs[log.ApplicationUserId].LastEdit = log.ActionDate;
                officerKPIs[log.ApplicationUserId].LastEditCount = log.ValuesCount;
                officerKPIs[log.ApplicationUserId].Log = log.Notes;
                officerKPIs[log.ApplicationUserId].Year = log.Year;
                var totalAdd = totalUpdates.Where(x => x.ApplicationUserId == log.ApplicationUserId).FirstOrDefault();
                if (totalAdd != null)
                    officerKPIs[log.ApplicationUserId].TotalEditSum = totalAdd.Sum;
            }
            ViewBag.OfficersKPI = officerKPIs.Values.ToList();
            return View();
        }
    }
}