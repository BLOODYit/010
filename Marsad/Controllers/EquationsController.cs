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
using System.Globalization;
using System.Text.RegularExpressions;

namespace Marsad.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EquationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Equations
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? indicatorID)
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

            var equations = db.Equations.Include(e => e.Indicator).Include(e => e.Indicator.Bundle).AsQueryable();
            equations = SortParams(sortOrder, equations, searchString);
            if (indicatorID.HasValue)
                equations = equations.Where(x => x.IndicatorID == indicatorID.Value);
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
        public ActionResult Create(int indicatorID)
        {
            var indicator = db.Indicators.Find(indicatorID);
            if (indicator == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var bundles = db.Bundles.AsQueryable();
            bundles = bundles.Where(x => x.Indicators.Any());
            ViewBag.IndicatorID = new SelectList(indicator.Bundle.Indicators, "ID", "Name", indicator);
            ViewBag.BundleID = new SelectList(bundles, "ID", "Name", indicator.Bundle);
            ViewBag.Elements = db.Elements.ToDictionary(x => x.ID, x => x.Name);
            CultureInfo arSA = new CultureInfo("ar-SA");
            arSA.DateTimeFormat.Calendar = new HijriCalendar();
            ViewBag.CurrentHijriYear = arSA.Calendar.GetYear(DateTime.Now);
            return View();
        }

        // POST: Equations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,IndicatorID,EquationText")] Equation equation, int[] elementIds, int[] years)
        {
            if (years == null || years.Length == 0)
            {
                ModelState.AddModelError("years", "يجب اختيار سنة واحدة على الاقل");
            }
            else
            {
                years = years.Distinct().ToArray();
                Array.Sort(years);
                for (int i = 0; i < years.Length; i++)
                {
                    int year = years[i];
                    if (IsIndicatorYearExists(equation.IndicatorID, year))
                    {
                        ModelState.AddModelError("years", string.Format("سنة {0} مكررة", years[i]));
                    }
                }
            }
            if (!ValidateEquation(equation.EquationText))
            {
                ModelState.AddModelError("EquationText", "معادلة غير صالحة");
            }
            if (ModelState.IsValid)
            {
                foreach (int year in years)
                {
                    equation.EquationYears.Add(new EquationYear()
                    {
                        Year = year
                    });
                }
                db.Equations.Add(equation);
                var matches = Regex.Matches(equation.EquationText, @"\[(.*?)\]");
                HashSet<string> element_names = new HashSet<string>();
                foreach (Match match in matches)
                {
                    element_names.Add(match.Value.Replace("[", "").Replace("]", ""));
                }
                var elements = db.Elements.Where(x => element_names.Contains(x.Name)).ToList();
                foreach (var element in elements)
                {
                    equation.EquationElements.Add(new EquationElement()
                    {
                        Element = element,
                        ElementID = element.ID
                    });
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var indicator = db.Indicators.Find(equation.IndicatorID);
            if (indicator == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var bundles = db.Bundles.AsQueryable();
            bundles = bundles.Where(x=>x.Indicators.Any());
            ViewBag.IndicatorID = new SelectList(indicator.Bundle.Indicators, "ID", "Name", indicator);
            ViewBag.BundleID = new SelectList(bundles, "ID", "Name", indicator.Bundle);
            ViewBag.Elements = db.Elements.ToDictionary(x => x.ID, x => x.Name);
            CultureInfo arSA = new CultureInfo("ar-SA");
            arSA.DateTimeFormat.Calendar = new HijriCalendar();
            ViewBag.CurrentHijriYear = arSA.Calendar.GetYear(DateTime.Now);

            ViewBag.ElementIds = elementIds;
            ViewBag.Years = years;
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
            ViewBag.IndicatorID = new SelectList(db.Indicators, "ID", "Name", equation.IndicatorID);
            ViewBag.Elements = db.Elements.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.Years = equation.EquationYears.Select(x => x.Year).ToArray();
            return View(equation);
        }

        // POST: Equations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,IndicatorID,EquationText")] Equation equation, int[] elementIds, int[] years)
        {
            if (years == null || years.Length == 0)
            {
                ModelState.AddModelError("years", "يجب اختيار سنة واحدة على الاقل");
            }
            else
            {
                years = years.Distinct().ToArray();
                Array.Sort(years);
                for (int i = 0; i < years.Length; i++)
                {
                    int year = years[i];
                    if (IsIndicatorYearExists(equation.IndicatorID, year, equation.ID))
                    {
                        ModelState.AddModelError("years", string.Format("سنة {0} مكررة", years[i]));
                    }
                }
            }
            if (!ValidateEquation(equation.EquationText))
            {
                ModelState.AddModelError("EquationText", "معادلة غير صالحة");
            }
            if (ModelState.IsValid)
            {
                db.Entry(equation).State = EntityState.Modified;
                var _equation = db.Equations.Include(x => x.EquationElements).Include(x => x.EquationYears).Where(x => x.ID == equation.ID).First();
                equation.EquationElements = _equation.EquationElements;


                db.EquationYears.RemoveRange(equation.EquationYears);
                db.EquationElements.RemoveRange(equation.EquationElements);

                foreach (int year in years)
                {
                    equation.EquationYears.Add(new EquationYear()
                    {
                        Year = year
                    });
                }
                var matches = Regex.Matches(equation.EquationText, @"\[(.*?)\]");
                HashSet<string> element_names = new HashSet<string>();
                foreach (Match match in matches)
                {
                    element_names.Add(match.Value.Replace("[", "").Replace("]", ""));
                }
                var elements = db.Elements.Where(x => element_names.Contains(x.Name)).ToList();
                foreach (var element in elements)
                {
                    equation.EquationElements.Add(new EquationElement()
                    {
                        Element = element,
                        ElementID = element.ID
                    });
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IndicatorID = new SelectList(db.Indicators, "ID", "Name", equation.IndicatorID);
            ViewBag.Elements = db.Elements.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.ElementIds = elementIds;
            ViewBag.Years = years;
            equation.Indicator = db.Indicators.Find(equation.IndicatorID);
            foreach (var year in years)
                equation.EquationYears.Add(new EquationYear() { Year = year });
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
                equations = equations.Where(x =>
                x.Indicator.Code.ToString().Contains(searchString) ||
                x.Indicator.Name.Contains(searchString) ||
                x.Indicator.Bundle.Name.Contains(searchString)
                );
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.IndicatorIDSortParm = sortOrder == "IndicatorID" ? "IndicatorIDDesc" : "IndicatorID";
            ViewBag.IndicatorNameSortParm = sortOrder == "IndicatorName" ? "IndicatorNameDesc" : "IndicatorName";
            ViewBag.YearSortParm = sortOrder == "Year" ? "YearDesc" : "Year";
            ViewBag.EquationTextSortParm = sortOrder == "EquationText" ? "EquationTextDesc" : "EquationText";
            ViewBag.BundleNameSortParm = sortOrder == "BundleName" ? "BundleNameDesc" : "BundleName";


            switch (sortOrder)
            {
                case "IndicatorIDDesc":
                    equations = equations.OrderByDescending(s => s.Indicator.Code);
                    break;
                case "IndicatorID":
                    equations = equations.OrderBy(s => s.Indicator.Code);
                    break;
                case "IndicatorNameDesc":
                    equations = equations.OrderByDescending(s => s.Indicator.Name);
                    break;
                case "IndicatorName":
                    equations = equations.OrderBy(s => s.Indicator.Name);
                    break;
                case "EquationTextDesc":
                    equations = equations.OrderByDescending(s => s.EquationText);
                    break;
                case "EquationText":
                    equations = equations.OrderBy(s => s.EquationText);
                    break;
                case "BundleNameDesc":
                    equations = equations.OrderByDescending(s => s.Indicator.Bundle.Name);
                    break;
                case "BundleName":
                    equations = equations.OrderBy(s => s.Indicator.Bundle.Name);
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

        private bool ValidateEquation(string equationText)
        {
            if (string.IsNullOrWhiteSpace(equationText))
                return false;
            var matches = Regex.Matches(equationText, @"\[(.*?)\]");
            foreach (Match match in matches)
            {
                equationText = equationText.Replace(match.Value, "@p0");
            }
            try
            {
                var result = db.Database.ExecuteSqlCommand("SELECT " + equationText, 1);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool IsIndicatorYearExists(int indicatorId, int year, int equationId = 0)
        {
            var equationIds = db.Equations.Where(x => x.ID != equationId).Where(x => x.IndicatorID == indicatorId).Select(x => x.ID).ToList();
            if (equationIds != null)
            {
                return db.EquationYears.Where(x => equationIds.Contains(x.EquationID) && x.Year == year).Any();
            }
            return false;
        }
    }


}

