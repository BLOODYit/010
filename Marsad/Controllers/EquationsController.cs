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
    public class EquationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Equations
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

            var equations = db.Equations.Include(e => e.Indicator).AsQueryable();
            equations = SortParams(sortOrder, equations, searchString);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(equations.ToPagedList(pageNumber, pageSize));
        }

        // GET: Equations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equation equation = db.Equations.Find(id);
            if (equation == null)
            {
                return HttpNotFound();
            }
            return View(equation);
        }

        // GET: Equations/Create
        public ActionResult Create()
        {
            ViewBag.IndicatorID = new SelectList(db.Indicators, "ID", "Code");
            return View();
        }

        // POST: Equations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,IndicatorID,Year,EquationText")] Equation equation)
        {
            if (ModelState.IsValid)
            {
                db.Equations.Add(equation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IndicatorID = new SelectList(db.Indicators, "ID", "Code", equation.IndicatorID);
            return View(equation);
        }

        // GET: Equations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equation equation = db.Equations.Find(id);
            if (equation == null)
            {
                return HttpNotFound();
            }
            ViewBag.IndicatorID = new SelectList(db.Indicators, "ID", "Code", equation.IndicatorID);
            return View(equation);
        }

        // POST: Equations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,IndicatorID,Year,EquationText")] Equation equation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IndicatorID = new SelectList(db.Indicators, "ID", "Code", equation.IndicatorID);
            return View(equation);
        }

        // GET: Equations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equation equation = db.Equations.Find(id);
            if (equation == null)
            {
                return HttpNotFound();
            }
            return View(equation);
        }

        // POST: Equations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Equation equation = db.Equations.Find(id);
            db.Equations.Remove(equation);
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

        private IQueryable<Equation> SortParams(string sortOrder, IQueryable<Equation> equations, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
                equations = equations.Where(x => x.Indicator.Name.Contains(searchString));
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.IndicatorIDSortParm = sortOrder == "IndicatorID" ? "IndicatorIDDesc" : "IndicatorID";
            ViewBag.YearSortParm = sortOrder == "Year" ? "YearDesc" : "Year";
            ViewBag.EquationTextSortParm = sortOrder == "EquationText" ? "EquationTextDesc" : "EquationText";



            switch (sortOrder)
            {
                case "IndicatorIDDesc":
                    equations = equations.OrderByDescending(s => s.Indicator.Name);
                    break;
                case "IndicatorID":
                    equations = equations.OrderBy(s => s.Indicator.Name);
                    break;
                case "YearDesc":
                    equations = equations.OrderByDescending(s => s.Year);
                    break;
                case "Year":
                    equations = equations.OrderBy(s => s.Year);
                    break;
                case "EquationTextDesc":
                    equations = equations.OrderByDescending(s => s.EquationText);
                    break;
                case "EquationText":
                    equations = equations.OrderBy(s => s.EquationText);
                    break;

                case "IDDesc":
                    equations = equations.OrderByDescending(s => s.ID);
                    break;
                default:
                    equations = equations.OrderBy(s => s.ID);
                    break;
            }
            return equations;
        }

    }


}

