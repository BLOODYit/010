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
    public class IndicatorTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: IndicatorTypes
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

            var indicatorTypes = db.IndicatorTypes.AsQueryable();
            indicatorTypes = SortParams(sortOrder, indicatorTypes, searchString);
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(indicatorTypes.ToPagedList(pageNumber, pageSize));
        }

        // GET: IndicatorTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorType indicatorType = db.IndicatorTypes.Find(id);
            if (indicatorType == null)
            {
                return HttpNotFound();
            }
            return View(indicatorType);
        }

        // GET: IndicatorTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IndicatorTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] IndicatorType indicatorType)
        {
            if (ModelState.IsValid)
            {
                db.IndicatorTypes.Add(indicatorType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(indicatorType);
        }

        // GET: IndicatorTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorType indicatorType = db.IndicatorTypes.Find(id);
            if (indicatorType == null)
            {
                return HttpNotFound();
            }
            return View(indicatorType);
        }

        // POST: IndicatorTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] IndicatorType indicatorType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indicatorType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(indicatorType);
        }

        // GET: IndicatorTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorType indicatorType = db.IndicatorTypes.Find(id);
            if (indicatorType == null)
            {
                return HttpNotFound();
            }
            return View(indicatorType);
        }

        // POST: IndicatorTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IndicatorType indicatorType = db.IndicatorTypes.Find(id);
            db.IndicatorTypes.Remove(indicatorType);
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

        private IQueryable<IndicatorType> SortParams(string sortOrder, IQueryable<IndicatorType> indicatorTypes, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
                indicatorTypes = indicatorTypes.Where(x => x.Name.Contains(searchString));
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";



            switch (sortOrder)
            {
                case "NameDesc":
                    indicatorTypes = indicatorTypes.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    indicatorTypes = indicatorTypes.OrderBy(s => s.Name);
                    break;

                case "IDDesc":
                    indicatorTypes = indicatorTypes.OrderByDescending(s => s.ID);
                    break;
                default:
                    indicatorTypes = indicatorTypes.OrderBy(s => s.ID);
                    break;
            }
            return indicatorTypes;
        }

    }


}

