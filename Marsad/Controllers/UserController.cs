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
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var store = new UserStore<ApplicationUser>(db);
                var manger = new UserManager<ApplicationUser>(store);
                
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                manger.Create(user, model.Password);
                return RedirectToAction("Index");
                //if (result.Succeeded)
                //{
                //    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                //    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                //    // Send an email with this link
                //    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                //    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                //    return RedirectToAction("Index", "Home");
                //}

            }

            // If we got this far, something failed, redisplay form
            return View(model);
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

    }
}