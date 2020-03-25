using Marsad.Models;
using Marsad.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var officerKPI = db.ElementValues
                .Where(x => officerIds.Contains(x.ApplicationUserID))
                .GroupBy(x => new { x.ApplicationUser })
                .Select(x => new OfficerKPI { ApplicationUser = x.Key.ApplicationUser, Count = x.Count(), LastUpdated = x.Max(y => y.CreatedAt) }).ToList();
            ViewBag.OfficersKPI = officerKPI;
            return View();
        }

    }
}