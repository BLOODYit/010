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
    public class GeoAreasController : BaseController
    {
        // GET: GeoAreas
        public ActionResult Index(string type, string sortOrder, string currentFilter, string searchString, int? page)
        {

            type = SetType(type);
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

            var geoAreas = db.GeoAreas.Where(x => x.Type.Equals(type)).Include(g => g.ParentGeoArea).AsQueryable();
            geoAreas = SortParams(sortOrder, geoAreas, searchString);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(geoAreas.ToPagedList(pageNumber, pageSize));
        }

        // GET: GeoAreas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeoArea geoArea = db.GeoAreas.Find(id);
            if (geoArea == null)
            {
                return HttpNotFound();
            }

            SetType(geoArea.Type);
            return View(geoArea);
        }

        // GET: GeoAreas/Create
        public ActionResult Create(string type)
        {
            type = SetType(type);
            ViewBag.GeoAreas = db.GeoAreas.ToList();
            return View();
        }

        // POST: GeoAreas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Name,GeoAreaID,Type")] GeoArea geoArea)
        {
            if (ModelState.IsValid)
            {
                db.GeoAreas.Add(geoArea);
                db.SaveChanges();
                Log(LogAction.Create, geoArea);
                return RedirectToAction("Index", new { Type = geoArea.Type });
            }
            string type = SetType(geoArea.Type);
            ViewBag.GeoAreas = db.GeoAreas.ToList();
            return View(geoArea);
        }

        // GET: GeoAreas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeoArea geoArea = db.GeoAreas.Find(id);
            if (geoArea == null)
            {
                return HttpNotFound();
            }
            string type = SetType(geoArea.Type);
            ViewBag.GeoAreas = db.GeoAreas.ToList();
            return View(geoArea);
        }

        // POST: GeoAreas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Name,GeoAreaID,Type")] GeoArea geoArea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(geoArea).State = EntityState.Modified;
                db.SaveChanges();
                Log(LogAction.Update, geoArea);
                return RedirectToAction("Index", new { Type = geoArea.Type });
            }
            string type = SetType(geoArea.Type);
            ViewBag.GeoAreas = db.GeoAreas.ToList();
            return View(geoArea);
        }

        // GET: GeoAreas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeoArea geoArea = db.GeoAreas.Find(id);
            if (geoArea == null)
            {
                return HttpNotFound();
            }
            SetType(geoArea.Type);
            return View(geoArea);
        }

        // POST: GeoAreas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GeoArea geoArea = db.GeoAreas.Find(id);
            GeoArea _geoArea = new GeoArea() { ID = id, Name = geoArea.Name };
            if (!geoArea.Type.Equals("Kingdom"))
            {
                db.GeoAreas.Remove(geoArea);
                db.SaveChanges();
                Log(LogAction.Delete, _geoArea);
            }
            return RedirectToAction("Index", new { Type = geoArea.Type });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private IQueryable<GeoArea> SortParams(string sortOrder, IQueryable<GeoArea> geoAreas, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
                geoAreas = geoAreas.Where(x => x.Name.Contains(searchString) || x.ParentGeoArea.Name.Contains(searchString));
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.CodeSortParm = sortOrder == "Code" ? "CodeDesc" : "Code";
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.GeoAreaIDSortParm = sortOrder == "GeoAreaID" ? "GeoAreaIDDesc" : "GeoAreaID";
            ViewBag.TypeSortParm = sortOrder == "Type" ? "TypeDesc" : "Type";



            switch (sortOrder)
            {
                case "CodeDesc":
                    geoAreas = geoAreas.OrderByDescending(s => s.Code);
                    break;
                case "Code":
                    geoAreas = geoAreas.OrderBy(s => s.Code);
                    break;
                case "NameDesc":
                    geoAreas = geoAreas.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    geoAreas = geoAreas.OrderBy(s => s.Name);
                    break;
                case "GeoAreaIDDesc":
                    geoAreas = geoAreas.OrderByDescending(s => s.ParentGeoArea.Name);
                    break;
                case "GeoAreaID":
                    geoAreas = geoAreas.OrderBy(s => s.ParentGeoArea.Name);
                    break;
                case "TypeDesc":
                    geoAreas = geoAreas.OrderByDescending(s => s.Type);
                    break;
                case "Type":
                    geoAreas = geoAreas.OrderBy(s => s.Type);
                    break;

                case "IDDesc":
                    geoAreas = geoAreas.OrderByDescending(s => s.ID);
                    break;
                default:
                    geoAreas = geoAreas.OrderBy(s => s.ID);
                    break;
            }
            return geoAreas;
        }

        private string SetType(string type)
        {
            if (string.IsNullOrWhiteSpace(type) || !GeoArea.Types.ContainsKey(type))
            {
                type = "Region";
            }
            ViewBag.Type = type;
            ViewBag.TypeName = GeoArea.Types[type];
            ViewBag.Parent = GeoArea.GetParentName(type);
            ViewBag.ParentName = GeoArea.Types[GeoArea.GetParentName(type)];
            return type;
        }
    }


}

