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

namespace Marsad.Controllers
{
    public class BundlesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Bundles
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bundles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Name,Description")] Bundle bundle)
        {
            if (ModelState.IsValid)
            {
                db.Bundles.Add(bundle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bundle);
        }

        // GET: Bundles/Edit/5
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Name,Description")] Bundle bundle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bundle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bundle);
        }

        // GET: Bundles/Delete/5
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bundle bundle = db.Bundles.Find(id);
            db.Bundles.Remove(bundle);
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

        private IQueryable<Bundle> SortParams(string sortOrder, IQueryable<Bundle> bundles, string searchString)
        {
            if (!string.IsNullOrWhiteSpace(searchString))
                bundles = bundles.Where(x => x.Name.Contains(searchString) || x.Code.Contains(searchString));
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.CodeSortParm = sortOrder == "Code" ? "CodeDesc" : "Code";
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.DescriptionSortParm = sortOrder == "Description" ? "DescriptionDesc" : "Description";

            switch (sortOrder)
            {

                case "CodeDesc":
                    bundles = bundles.OrderByDescending(s => s.Code);
                    break;
                case "Code":
                    bundles = bundles.OrderBy(s => s.Code);
                    break;
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

    }


}

