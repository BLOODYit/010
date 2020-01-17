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
    public class PeriodsController : BaseController
    {

        // GET: Periods
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

            var periods = db.Periods.AsQueryable();
            periods = SortParams(sortOrder, periods, searchString);
            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return View(periods.ToPagedList(pageNumber, pageSize));
        }

        // GET: Periods/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Period period = db.Periods.Find(id);
            if (period == null)
            {
                return HttpNotFound();
            }
            return View(period);
        }

        // GET: Periods/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Periods/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Year,Month,Day")] Period period)
        {
            if (ModelState.IsValid)
            {
                db.Periods.Add(period);
                db.SaveChanges();
                Log(LogAction.Create, period);
                return RedirectToAction("Index");
            }

            return View(period);
        }

        // GET: Periods/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Period period = db.Periods.Find(id);
            if (period == null)
            {
                return HttpNotFound();
            }
            return View(period);
        }

        // POST: Periods/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Year,Month,Day")] Period period)
        {
            if (ModelState.IsValid)
            {
                db.Entry(period).State = EntityState.Modified;
                db.SaveChanges();
                Log(LogAction.Update, period);
                return RedirectToAction("Index");
            }
            return View(period);
        }

        // GET: Periods/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Period period = db.Periods.Find(id);
            if (period == null)
            {
                return HttpNotFound();
            }
            return View(period);
        }

        // POST: Periods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Period period = db.Periods.Find(id);
            Period _period = new Period() { ID = id, Name = period.Name };
            db.Periods.Remove(period);
            db.SaveChanges();
            Log(LogAction.Delete, _period);
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

        private IQueryable<Period> SortParams(string sortOrder, IQueryable<Period> periods, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
                periods = periods.Where(x => x.Name.Contains(searchString));
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.YearSortParm = sortOrder == "Year" ? "YearDesc" : "Year";
            ViewBag.MonthSortParm = sortOrder == "Month" ? "MonthDesc" : "Month";
            ViewBag.DaySortParm = sortOrder == "Day" ? "DayDesc" : "Day";



            switch (sortOrder)
            {
                case "NameDesc":
                    periods = periods.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    periods = periods.OrderBy(s => s.Name);
                    break;
                case "YearDesc":
                    periods = periods.OrderByDescending(s => s.Year);
                    break;
                case "Year":
                    periods = periods.OrderBy(s => s.Year);
                    break;
                case "MonthDesc":
                    periods = periods.OrderByDescending(s => s.Month);
                    break;
                case "Month":
                    periods = periods.OrderBy(s => s.Month);
                    break;
                case "DayDesc":
                    periods = periods.OrderByDescending(s => s.Day);
                    break;
                case "Day":
                    periods = periods.OrderBy(s => s.Day);
                    break;

                case "IDDesc":
                    periods = periods.OrderByDescending(s => s.ID);
                    break;
                default:
                    periods = periods.OrderBy(s => s.ID);
                    break;
            }
            return periods;
        }

    }


}

