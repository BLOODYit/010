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
using Microsoft.AspNet.Identity;

namespace Marsad.Controllers
{
    [Authorize(Roles ="Admin,Officer")]
    public class BundlesController : BaseController
    {        
        // GET: Bundles
        [Authorize(Roles ="Admin")]
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

            var bundles = db.Bundles.AsQueryable();
            bundles = SortParams(sortOrder, bundles, searchString);
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(bundles.ToPagedList(pageNumber, pageSize));
        }

        // GET: Bundles/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bundle bundle = db.Bundles.Find(id);
            if (bundle == null)
            {
                return HttpNotFound();
            }
            return View(bundle);
        }

        // GET: Bundles/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bundles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Name,Description,Color")] Bundle bundle)
        {
            if (ModelState.IsValid)
            {
                db.Bundles.Add(bundle);
                db.SaveChanges();
                Log(LogAction.Create, bundle);
                return RedirectToAction("Index");
            }

            return View(bundle);
        }

        // GET: Bundles/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bundle bundle = db.Bundles.Find(id);
            if (bundle == null)
            {
                return HttpNotFound();
            }
            return View(bundle);
        }

        // POST: Bundles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Name,Description,Color")] Bundle bundle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bundle).State = EntityState.Modified;
                db.SaveChanges();
                Log(LogAction.Update, bundle);
                return RedirectToAction("Index");
            }
            return View(bundle);
        }

        // GET: Bundles/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bundle bundle = db.Bundles.Find(id);
            if (bundle == null)
            {
                return HttpNotFound();
            }
            return View(bundle);
        }

        // POST: Bundles/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bundle bundle = db.Bundles.Find(id);
            Bundle _bundle = new Bundle() { ID = id, Name = bundle.Name };
            db.Bundles.Remove(bundle);
            db.SaveChanges();
            Log(LogAction.Delete, _bundle);
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

        private IQueryable<Bundle> SortParams(string sortOrder, IQueryable<Bundle> bundles, string searchString)
        {
            if (!string.IsNullOrWhiteSpace(searchString))
                bundles = bundles.Where(x => x.Name.Contains(searchString) || x.ID.ToString().Equals(searchString));
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.CodeSortParm = sortOrder == "Code" ? "CodeDesc" : "Code";
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.DescriptionSortParm = sortOrder == "Description" ? "DescriptionDesc" : "Description";

            switch (sortOrder)
            {
                case "NameDesc":
                    bundles = bundles.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    bundles = bundles.OrderBy(s => s.Name);
                    break;
                case "DescriptionDesc":
                    bundles = bundles.OrderByDescending(s => s.Description);
                    break;
                case "Description":
                    bundles = bundles.OrderBy(s => s.Description);
                    break;
                default:
                    bundles = bundles.OrderBy(s => s.ID);
                    break;
            }
            return bundles;
        }

        [HttpGet]
        public JsonResult GetIndicators(int id)
        {
            var indicators = db.Indicators.Where(x => x.BundleID == id).Select(x => new { x.ID, x.Name }).ToList();
            return Json(new { success = true, data = indicators }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetIndicatorsWithValues(int id)
        {
            bool isAdmin = User.IsInRole("Admin");            
            var userId = User.Identity.GetUserId();
            var bundleUsersIds = db.Bundles.Where(x => x.Users.Where(y => y.Id == userId).Any()).Select(x => x.ID).ToList();
            var indicatorUsersIds = db.Indicators.Where(x => x.Users.Where(y => y.Id == userId).Any() || bundleUsersIds.Contains(x.BundleID)).Select(x => x.ID).ToList();
            var indicators = db.Indicators.Where(x=>x.Equations.Any()).Where(x => x.BundleID == id && (indicatorUsersIds.Contains(x.ID) || isAdmin)).Select(x => new { x.ID, x.Name }).ToList();
            return Json(new { success = true, data = indicators }, JsonRequestBehavior.AllowGet);
        }
        
    }


}

