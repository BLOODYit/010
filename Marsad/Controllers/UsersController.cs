﻿using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Marsad.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace Marsad.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db;
        private UserStore<ApplicationUser> userStore;
        private ApplicationUserManager userManager;
        private RoleStore<IdentityRole> roleStore;
        private RoleManager<IdentityRole> roleManager;

        public UsersController()
        {
            db = new ApplicationDbContext();
            userStore = new UserStore<ApplicationUser>(db);
            userManager = new ApplicationUserManager(userStore);
            roleStore = new RoleStore<IdentityRole>(db);
            roleManager = new RoleManager<IdentityRole>(roleStore);
        }
        // GET: Indicators        
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
            ViewBag.Roles = roleManager.Roles.ToDictionary(x=>x.Id,x=>x.Name);

            var users = userManager.Users;
            users = SortParams(sortOrder, users, searchString);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
        }

        // GET: Indicators/Details/5        
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user =  userManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Indicators/Create        
        public ActionResult Create()
        {
            ViewBag.BundleID = new SelectList(db.Bundles, "ID", "Name");
            ViewBag.IndicatorID = new SelectList(db.Indicators, "ID", "Name");
            ViewBag.RoleID = new SelectList(roleManager.Roles, "Id", "Name");
            return View();
        }

        // POST: Indicators/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterViewModel newUser, int[] indicatorIds, int[] bundleIds)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Name = newUser.Name,
                    UserName = newUser.UserName,
                    Email = newUser.UserName
                };
                if (indicatorIds != null)
                {
                    var indicators = db.Indicators.Where(x => indicatorIds.Contains(x.ID)).ToList();
                    foreach (var indicator in indicators)
                    {
                        user.Indicators.Add(indicator);
                    }
                }

                if (bundleIds != null)
                {
                    var bundles = db.Bundles.Where(x => bundleIds.Contains(x.ID)).ToList();
                    foreach (var bundle in bundles)
                    {
                        user.Bundles.Add(bundle);
                    }
                }
                userManager.Create(user, newUser.Password);
                user = userManager.FindByName(user.UserName);
                userManager.AddToRole(user.Id, newUser.RoleID);
                return RedirectToAction("Index");
            }

            ViewBag.BundleID = new SelectList(db.Bundles, "ID", "Name");
            ViewBag.IndicatorID = new SelectList(db.Indicators, "ID", "Name");
            ViewBag.RoleID = new SelectList(roleManager.Roles, "Id", "Name");
            return View(newUser);
        }

        // GET: Indicators/Edit/5        
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = userManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var roleId = user.Roles.FirstOrDefault().RoleId;
            var roleName = roleManager.Roles.Where(x => x.Id == roleId).FirstOrDefault().Name;
            ViewBag.BundleID = new SelectList(db.Bundles, "ID", "Name", user.Bundles.Select(x => x.ID).ToArray());
            ViewBag.IndicatorID = new SelectList(db.Indicators, "ID", "Name", user.Indicators.Select(x => x.ID).ToArray());
            ViewBag.RoleID = new SelectList(roleManager.Roles, "Id", "Name", roleName);
            return View(new RegisterViewModel()
            {
                Email = string.IsNullOrWhiteSpace(user.Email) ? user.UserName : user.Email,
                EntityID = user.EntityID,
                Name = user.Name,
                UserName = user.UserName,
                RoleID = roleName
            });
        }

        // POST: Indicators/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Name,UserName,Email,EntityID,RoleID,Password,ConfirmPassword")] EditUserViewModel oldUser, int[] indicatorIds, int[] bundlesIds, string Id)
        {
            var user = userManager.FindById(Id);
            if (ModelState.IsValid)
            {

                user.Name = oldUser.Name;
                user.Email = oldUser.Email;
                user.EntityID = oldUser.EntityID;
                //indicators
                //Bundles
                userManager.Update(user);
                var roleIds = user.Roles.Select(x => x.RoleId).ToList();
                var roleNames = roleManager.Roles.Where(x => roleIds.Contains(x.Id)).Select(x => x.Name).ToArray();
                userManager.RemoveFromRoles(user.Id, roleNames);
                userManager.AddToRole(user.Id, oldUser.RoleID);
                if (!string.IsNullOrEmpty(oldUser.Password))
                {
                    var token = userManager.GeneratePasswordResetToken(user.Id);
                    userManager.ResetPassword(user.Id, token, oldUser.Password);
                }
                return RedirectToAction("Index");
            }
            ViewBag.BundleID = new SelectList(db.Bundles, "ID", "Name", user.Bundles.Select(x => x.ID).ToArray());
            ViewBag.IndicatorID = new SelectList(db.Indicators, "ID", "Name", user.Indicators.Select(x => x.ID).ToArray());
            ViewBag.RoleID = new SelectList(roleManager.Roles, "Id", "Name", oldUser.RoleID);
            return View(oldUser);
        }

        // GET: Indicators/Delete/5        
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = userManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Indicators/Delete/5        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser user = userManager.FindById(id);
            userManager.Delete(user);
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
                users = users.Where(x => x.Name.Contains(searchString) || x.UserName.ToString().Contains(searchString) || x.Email.ToString().Contains(searchString));

            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "EmailDesc" : "Email";
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.UserNameSortParm = sortOrder == "UserName" ? "UserNameDesc" : "UserName";           

            switch (sortOrder)
            {
                case "EmailDesc":
                    users = users.OrderByDescending(s => s.Email);
                    break;
                case "Email":
                    users = users.OrderBy(s => s.Email);
                    break;
                case "NameDesc":
                    users = users.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    users = users.OrderBy(s => s.Name);
                    break;
                case "UserNameDesc":
                    users = users.OrderByDescending(s => s.UserName);
                    break;
                case "UserName":
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
