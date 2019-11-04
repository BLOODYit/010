using Marsad.Models;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Marsad.Models.ViewModels;
using System.Security.Claims;
using System.Net;

namespace Marsad.Controllers
{
    public class UserController : Controller
    {

        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public RoleManager<IdentityRole> RoleManager
        {
            get
            {
                var store = new RoleStore<IdentityRole>(db);
                return new RoleManager<IdentityRole>(store);
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var users = db.Users.AsQueryable();
            users = SortParams(sortOrder, users, searchString);
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(users.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            ViewBag.Roles = RoleManager.Roles.ToList();            
            ViewBag.Claims = GetClaims();

            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public  ActionResult Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {               
                return RedirectToAction("Index");
            }
            ViewBag.Roles = RoleManager.Roles.ToList();
            ViewBag.Claims = GetClaims();
            return View(model);
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = UserManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            
            RegisterViewModel registerViewModel = new RegisterViewModel() {
                Email=user.Email,
                EntityName=user.EntityName,
                UserName =user.UserName
            };
            ViewBag.Roles = RoleManager.Roles.ToList();
            ViewBag.Claims = GetClaims();
            ViewBag.UserRoles = user.Roles;
            ViewBag.UserClaims = user.Claims;
            return View(registerViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {               
                return RedirectToAction("Index");
            }


            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            db.Users.Remove(user);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        private IQueryable<ApplicationUser> SortParams(string sortOrder, IQueryable<ApplicationUser> users, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
                users = users.Where(x => x.UserName.Contains(searchString));
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";



            switch (sortOrder)
            {
                case "NameDesc":
                    users = users.OrderByDescending(s => s.UserName);
                    break;
                case "Name":
                    users = users.OrderBy(s => s.UserName);
                    break;

                case "IDDesc":
                    users = users.OrderByDescending(s => s.Id);
                    break;
                default:
                    users = users.OrderBy(s => s.Id);
                    break;
            }
            return users;
        }

        private Dictionary<string, string> GetClaims()
        {
            return new Dictionary<string, string>() {
                {"Query","الإستعلام"},
                {"Reports","التقارير"},
                {"Updates","التحديث"},
                {"Bundles","إدارة الحزم"},
                {"Indicators","إدارة المؤشرات"},
                {"Elements","إدارة العناصر والقيم"},
                {"IndicatorGroups","إدارة مجموعات المؤشات"},
                {"DataSources","إدارة مصادر البيانات"},
                {"Cases","إدارة قضايا التنمية"},
                {"Equations","إدارة المعادلات الحسابية"},
                {"GeoAreas","إدارة النظاقات الجغرافية"},
                {"Security","إدارة انظمة أمن البيانات"},
                {"Log","إدارة انظمة مراجعة البيانات"},
            };
        }
    }
}