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
    public class CasesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cases
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

            var cases = db.Cases.AsQueryable();
            cases = SortParams(sortOrder, cases, searchString);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(cases.ToPagedList(pageNumber, pageSize));
        }

        // GET: Cases/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = db.Cases.Find(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            return View(@case);
        }

        // GET: Cases/Create
        public ActionResult Create()
        {
            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name");
            ViewBag.Indicators = db.Indicators.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.Entities = db.Entities.ToDictionary(x => x.ID, x => x.Name);
            return View();
        }

        // POST: Cases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Description,Year,PeriodID")] Case @case, int[] indicatorIds, int[] entityIds)
        {
            if (ModelState.IsValid)
            {
                db.Cases.Add(@case);
                @case.Entities.AddRange(db.Entities.Where(x => entityIds.Contains(x.ID)));
                @case.Indicators.AddRange(db.Indicators.Where(x => indicatorIds.Contains(x.ID)));
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name", @case.PeriodID);
            ViewBag.Indicators = db.Indicators.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.Entities = db.Entities.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.indicatorIds = indicatorIds;
            ViewBag.entityIds = entityIds;
            return View(@case);
        }

        // GET: Cases/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = db.Cases.Include(x=>x.Indicators).Include(x=>x.Entities).Where(x=>x.ID==id).FirstOrDefault();
            if (@case == null)
            {
                return HttpNotFound();
            }
            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name", @case.PeriodID);
            ViewBag.Indicators = db.Indicators.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.Entities = db.Entities.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.indicatorIds = @case.Indicators.Select(x=>x.ID).ToArray();
            ViewBag.entityIds = @case.Entities.Select(x => x.ID).ToArray();
            return View(@case);
        }

        // POST: Cases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,Year,PeriodID")] Case @case,int[] indicatorIds, int[] entityIds)
        {
            if (ModelState.IsValid)
            {                
                db.Entry(@case).State = EntityState.Modified;
                var _case = db.Cases.Include(x => x.Indicators).Include(x => x.Entities).Where(x => x.ID == @case.ID).FirstOrDefault();
                @case.Entities = _case.Entities;
                @case.Indicators = _case.Indicators;
                @case.Entities.Clear();
                @case.Indicators.Clear();
                @case.Entities.AddRange(db.Entities.Where(x => entityIds.Contains(x.ID)));
                @case.Indicators.AddRange(db.Indicators.Where(x => indicatorIds.Contains(x.ID)));
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name", @case.PeriodID);
            ViewBag.Indicators = db.Indicators.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.Entities = db.Entities.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.indicatorIds = indicatorIds;
            ViewBag.entityIds = entityIds;
            return View(@case);
        }

        // GET: Cases/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Case @case = db.Cases.Find(id);
            if (@case == null)
            {
                return HttpNotFound();
            }
            return View(@case);
        }

        // POST: Cases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Case @case = db.Cases.Find(id);
            db.Cases.Remove(@case);
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

        private IQueryable<Case> SortParams(string sortOrder, IQueryable<Case> cases, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
                cases = cases.Where(x => x.Name.Contains(searchString));
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.DescriptionSortParm = sortOrder == "Description" ? "DescriptionDesc" : "Description";
            ViewBag.YearSortParm = sortOrder == "Year" ? "YearDesc" : "Year";
            ViewBag.PeriodIDSortParm = sortOrder == "PeriodID" ? "PeriodIDDesc" : "PeriodID";



            switch (sortOrder)
            {
                case "NameDesc":
                    cases = cases.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    cases = cases.OrderBy(s => s.Name);
                    break;
                case "DescriptionDesc":
                    cases = cases.OrderByDescending(s => s.Description);
                    break;
                case "Description":
                    cases = cases.OrderBy(s => s.Description);
                    break;
                case "YearDesc":
                    cases = cases.OrderByDescending(s => s.Year);
                    break;
                case "Year":
                    cases = cases.OrderBy(s => s.Year);
                    break;
                case "PeriodIDDesc":
                    cases = cases.OrderByDescending(s => s.PeriodID);
                    break;
                case "PeriodID":
                    cases = cases.OrderBy(s => s.PeriodID);
                    break;

                case "IDDesc":
                    cases = cases.OrderByDescending(s => s.ID);
                    break;
                default:
                    cases = cases.OrderBy(s => s.ID);
                    break;
            }
            return cases;
        }

    }


}

