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
using Marsad.Models.ViewModels;

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
                return Json(new { redirectToUrl = Url.Action("Index", "UserGroups") });
                //return RedirectToAction("Index");
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
            UserGroupViewModel UGVM = new UserGroupViewModel();
            var claimsString = new List<string>();
            foreach (var claim in userGroup.Claims)
            {
                claimsString.Add(claim.Name);
            }
            UGVM.SelectedClaims = claimsString;
            UGVM.UserGroup = userGroup;
            if (userGroup == null)
            {
                return HttpNotFound();
            }

            return View(UGVM);
        }

        // POST: UserGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(/*[Bind(Include= "ID,Name,Claims")]*/ UserGroupViewModel userGroupVM)
        {
            if (ModelState.IsValid)
            {
                var store = new UserStore<ApplicationUser>(db);
                var manger = new UserManager<ApplicationUser>(store);
                var oldUG = db.UserGroups.Find(userGroupVM.UserGroup.ID);
                var oldClaims = oldUG.Claims.ToList();
                string[] selectedClaims = userGroupVM.SelectedClaims[0].Split(',');
                for (int i = 0; i < selectedClaims.Length; i++)
                {
                    userGroupVM.UserGroup.Claims.Add(new MyClaim(i.ToString(),selectedClaims[i]));
                }
               
                //var selectedClaims = userGroupVM.SelectedClaims;

                //Remove old claims from userGroup
                foreach (var claim in oldClaims)
                {
                    db.MyClaims.Remove(claim);
                }
                oldUG.Name = userGroupVM.UserGroup.Name;
                oldUG.Claims = userGroupVM.UserGroup.Claims;

                //get users assigned to userGroup
                var users_UG = db.Users.Where(u => u.UserGroupID == userGroupVM.UserGroup.ID).ToList();
                //Remove all IdentityClaims from users assigned to userGroup
                if (users_UG !=null)
                {
                    foreach (var u in users_UG)
                    {
                        var x = u.Id;
                        var claims = manger.GetClaims(x);
                        if (claims!=null)
                        {
                            foreach (var c in claims)
                            {
                                manger.RemoveClaim(u.Id, c);

                            }

                        }

                    }

                }
               
                //set new claims from userGroup to his users
                if (oldUG.Claims!=null&& users_UG!=null)
                {
                    foreach (var u in users_UG)
                    {
                        //foreach (var c in oldUG.Claims)
                        //{
                        //    var value = c.Name;
                        //    manger.AddClaim(u.Id, new Claim("Claim", value));

                        //}
                        foreach (var claim in selectedClaims)
                        {
                            manger.AddClaim(u.Id, new Claim("Claim", claim));
                        }

                    }

                }
                db.SaveChanges();
                //return Json(new { redirectToUrl = Url.Action("Index", "UserGroups") });
                return RedirectToAction("Index");

            }
            //var claimsString = new List<string>();
            //foreach (var claim in userGroup.Claims)
            //{
            //    claimsString.Add(claim.Name);
            //}
            //ViewBag.claims = claimsString;
            //return Json(new { redirectToUrl = Url.Action("Index", "UserGroups") });
            return View(userGroupVM);




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
           
            var users = db.Users.Where(u => u.UserGroupID == id).ToList();
            foreach (var u in users)
            {
                db.Users.Remove(u);
            }
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

