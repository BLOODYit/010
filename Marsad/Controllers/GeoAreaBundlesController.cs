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
    [Authorize(Roles = "Admin")]
    public class GeoAreaBundlesController : BaseController
    {
        // GET: GeoAreaBundles
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

            var geoAreaBundles = db.GeoAreas.Where(x => x.Type == "Bundle").Include(x => x.GeoAreaBundles).AsQueryable();
            geoAreaBundles = SortParams(sortOrder, geoAreaBundles, searchString);
            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return View(geoAreaBundles.ToPagedList(pageNumber, pageSize));
        }

        // GET: GeoAreaBundles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeoArea geoAreaBundle = db.GeoAreas.Include(x => x.GeoAreaBundles).Where(x => x.ID == id && x.Type == "Bundle").FirstOrDefault();
            if (geoAreaBundle == null)
            {
                return HttpNotFound();
            }
            return View(geoAreaBundle);
        }

        // GET: GeoAreaBundles/Create
        public ActionResult Create()
        {
            ViewBag.GeoAreaTypes = GeoArea.Types;
            ViewBag.GeoAreas = db.GeoAreas.Where(x => x.Type != "Bundle").ToList();
            return View();
        }

        // POST: GeoAreaBundles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Name,Type")] GeoArea geoAreaBundle, int[] geo_area_ids)
        {
            geoAreaBundle.Type = "Bundle";
            if (ModelState.IsValid)
            {
                geoAreaBundle.GeoAreaBundles = new List<GeoAreaBundle>();
                var childGeoAreas = db.GeoAreas.Where(x => geo_area_ids.Contains(x.ID)).ToList();
                foreach(var geoArea in childGeoAreas)
                {
                    geoAreaBundle.GeoAreaBundles.Add(new GeoAreaBundle() { 
                        ChildGeoAreaID=geoArea.ID,                        
                    });
                }                
                db.GeoAreas.Add(geoAreaBundle);
                db.SaveChanges();
                Log(LogAction.Create, geoAreaBundle);
                return RedirectToAction("Index");
            }

            ViewBag.GeoAreaTypes = GeoArea.Types;
            ViewBag.GeoAreas = db.GeoAreas.Where(x => x.Type != "Bundle").ToList();
            return View(geoAreaBundle);
        }

        // GET: GeoAreaBundles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeoArea geoAreaBundle = db.GeoAreas.Where(x => x.ID == id && x.Type == "Bundle").Include(x => x.GeoAreaBundles).FirstOrDefault();
            if (geoAreaBundle == null)
            {
                return HttpNotFound();
            }

            ViewBag.GeoAreaTypes = GeoArea.Types;
            ViewBag.GeoAreas = db.GeoAreas.Where(x => x.Type != "Bundle").ToList();
            return View(geoAreaBundle);
        }

        // POST: GeoAreaBundles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Name,Type")] GeoArea geoAreaBundle, int[] geo_area_ids)
        {
            if (ModelState.IsValid)
            {

                db.Entry(geoAreaBundle).State = EntityState.Modified;
                geoAreaBundle.GeoAreaBundles = db.GeoAreas.Where(x => x.ID == geoAreaBundle.ID).Include(x => x.GeoAreaBundles).FirstOrDefault().GeoAreaBundles;
                if (geoAreaBundle.GeoAreaBundles == null)
                    geoAreaBundle.GeoAreaBundles = new List<GeoAreaBundle>();
                geoAreaBundle.GeoAreaBundles.Clear();

                var childGeoAreas = db.GeoAreas.Where(x => geo_area_ids.Contains(x.ID)).ToList();
                foreach (var geoArea in childGeoAreas)
                {
                    geoAreaBundle.GeoAreaBundles.Add(new GeoAreaBundle()
                    {
                        ChildGeoAreaID = geoArea.ID,
                    });
                }

                db.SaveChanges();
                Log(LogAction.Update, geoAreaBundle);
                return RedirectToAction("Index");
            }
            GeoArea.Types.Reverse();
            ViewBag.GeoAreaTypes = GeoArea.Types;
            ViewBag.GeoAreas = db.GeoAreas.Where(x => x.Type != "Bundle").ToList();
            return View(geoAreaBundle);
        }

        // GET: GeoAreaBundles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeoArea geoAreaBundle = db.GeoAreas.Where(x => x.ID == id && x.Type == "Bundle").FirstOrDefault();
            if (geoAreaBundle == null)
            {
                return HttpNotFound();
            }
            return View(geoAreaBundle);
        }

        // POST: GeoAreaBundles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GeoArea geoAreaBundle = db.GeoAreas.Where(x => x.ID == id && x.Type == "Bundle").FirstOrDefault();
            GeoArea _geoAreaBundle = new GeoArea() { ID = id, Name = geoAreaBundle.Name, Type = geoAreaBundle.Type };
            db.GeoAreas.Remove(geoAreaBundle);
            db.SaveChanges();
            Log(LogAction.Delete, _geoAreaBundle);
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

        private IQueryable<GeoArea> SortParams(string sortOrder, IQueryable<GeoArea> geoAreaBundles, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
                geoAreaBundles = geoAreaBundles.Where(x => x.Name.Contains(searchString));
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.CodeSortParm = sortOrder == "Code" ? "CodeDesc" : "Code";
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";



            switch (sortOrder)
            {
                case "CodeDesc":
                    geoAreaBundles = geoAreaBundles.OrderByDescending(s => s.Code);
                    break;
                case "Code":
                    geoAreaBundles = geoAreaBundles.OrderBy(s => s.Code);
                    break;
                case "NameDesc":
                    geoAreaBundles = geoAreaBundles.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    geoAreaBundles = geoAreaBundles.OrderBy(s => s.Name);
                    break;

                case "IDDesc":
                    geoAreaBundles = geoAreaBundles.OrderByDescending(s => s.ID);
                    break;
                default:
                    geoAreaBundles = geoAreaBundles.OrderBy(s => s.ID);
                    break;
            }
            return geoAreaBundles;
        }

    }


}

