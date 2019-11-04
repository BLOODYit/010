using PagedList;
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
using System.Security.Claims;

namespace Marsad.Controllers
{
    public class UserGroupsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserGroups
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

            var userGroups = db.UserGroups.AsQueryable();
            userGroups = SortParams(sortOrder, userGroups, searchString);
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(userGroups.ToPagedList(pageNumber, pageSize));
        }

        // GET: UserGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserGroup userGroup = db.UserGroups.Find(id);
            if (userGroup == null)
            {
                return HttpNotFound();
            }
            return View(userGroup);
        }

        // GET: UserGroups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
      //  [ValidateAntiForgeryToken]
        public ActionResult Create( UserGroup userGroup)
        {
           // [Bind(Include = "ID,Name")]
            if (ModelState.IsValid)
            {
               
                db.UserGroups.Add(userGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userGroup);
        }

        // GET: UserGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserGroup userGroup = db.UserGroups.Find(id);
            if (userGroup == null)
            {
                return HttpNotFound();
            }
            return View(userGroup);
        }

        // POST: UserGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include= "ID,Name,Claims")] UserGroup userGroup)
        {
            if (ModelState.IsValid)
            {
                var store = new UserStore<ApplicationUser>(db);
                var manger = new UserManager<ApplicationUser>(store);
                var oldUG = db.UserGroups.Find(userGroup.ID);
                var oldClaims = oldUG.Claims.ToList();
                //Remove old claims from userGroup
                foreach (var claim in oldClaims)
                {
                    db.MyClaims.Remove(claim);
                }
                oldUG.Name = userGroup.Name;
                oldUG.Claims = userGroup.Claims;
                //get users assigned to userGroup
                               
                db.SaveChanges();
                return RedirectToAction("Index");

            }
              return View(userGroup);
    


        }

        // GET: UserGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserGroup userGroup = db.UserGroups.Find(id);
            if (userGroup == null)
            {
                return HttpNotFound();
            }
            return View(userGroup);
        }

        // POST: UserGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserGroup userGroup = db.UserGroups.Find(id);
            var guClaims = userGroup.Claims.ToList();
            foreach (var c in guClaims)
            {
                db.MyClaims.Remove(c);      
            }
            db.UserGroups.Remove(userGroup);
           
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

        private IQueryable<UserGroup> SortParams(string sortOrder, IQueryable<UserGroup> userGroups, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
                userGroups = userGroups.Where(x => x.Name.Contains(searchString));
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";



            switch (sortOrder)
            {
                case "NameDesc":
                    userGroups = userGroups.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    userGroups = userGroups.OrderBy(s => s.Name);
                    break;

                case "IDDesc":
                    userGroups = userGroups.OrderByDescending(s => s.ID);
                    break;
                default:
                    userGroups = userGroups.OrderBy(s => s.ID);
                    break;
            }
            return userGroups;
        }

       

    }


}

