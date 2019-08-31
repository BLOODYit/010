﻿using Marsad.Models;
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

            //var users = db.Users.Where(u=>u.UserName != "admin@emarsd.com").AsQueryable();
            var users = db.Users.AsQueryable();
            users = SortParams(sortOrder, users, searchString);
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(users.ToPagedList(pageNumber, pageSize));
        }
        
        public ActionResult Create()
        {
            UserViewModel userVM = new UserViewModel();
            var userGroups = db.UserGroups.ToList();
            var sl = userGroups.Select(s => new SelectListItem { Value = s.ID.ToString(),Text=s.Name })
                .ToList();
            userVM.UserGroups = userGroups;
            return View(userVM);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var store = new UserStore<ApplicationUser>(db);
                var manger = new UserManager<ApplicationUser>(store);
                string[] str = model.UserGroup.Split(':');
                int userGroupId =Convert.ToInt32(str[0]);             
                var tempUserGroup = db.UserGroups.Find(userGroupId);             
                var myClaims = tempUserGroup.Claims; 
                var user = new ApplicationUser { UserName = model.RegisterVM.Email, Email = model.RegisterVM.Email };
                await manger.CreateAsync(user, model.RegisterVM.Password);
                tempUserGroup.ApplicationUsers.Add(user);
                db.SaveChanges();
                var userId = user.Id;
                foreach (var c in myClaims)
                {
                    var value = c.Name;
                    manger.AddClaim(userId, new Claim("Claim", value));

                }
                return RedirectToAction("Index");

              

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(string id)
        {
            UserViewModel userVM = new UserViewModel();
            var userGroups = db.UserGroups.ToList();
            var user = db.Users.Find(id);
            userVM.UserGroups = userGroups;
            userVM.UserName = user.Email;
            userVM.Id = id;
            userVM.UserGroup = user.UserGroup.Name;
            return View(userVM);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel userVM)
        {
            if (ModelState.IsValid)
            {
                var store = new UserStore<ApplicationUser>(db);
                var manger = new UserManager<ApplicationUser>(store);
                //string[] str = userVM.UserGroup.Split(':');
                //int userGroupId = Convert.ToInt32(str[0]);
                var tempUserGroup = db.UserGroups.Find(userVM.UGId);
                var newClaimss = tempUserGroup.Claims;
                //string userGroup = Request.Form["ddlUserGroups"].ToString();
                var userId = userVM.Id;
                var tempUser = db.Users.Find(userVM.Id);
                tempUser.UserGroup = tempUserGroup;
                var claims =  manger.GetClaims(userId);
                foreach (var c in claims)
                {
                    manger.RemoveClaim(userId, c);

                }
                foreach (var c in newClaimss)
                {
                    var value = c.Name;
                    manger.AddClaim(userId, new Claim("Claim", value));

                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            // If we got this far, something failed, redisplay form
            return View(userVM);
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

    }
}