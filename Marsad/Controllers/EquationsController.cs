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
    public class EquationsController : BaseController
    {
        // GET: Equations
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? indicatorID, int? bundleID)
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
            var indicators = db.Indicators.Where(x => x.Equations.Any()).Include(x => x.Bundle).Include(x => x.Equations).AsQueryable();
            indicators = SortParams(sortOrder, indicators, searchString);
            if (indicatorID.HasValue)
                indicators = indicators.Where(x => x.ID == indicatorID.Value);
            if (bundleID.HasValue)
                indicators = indicators.Where(x => x.BundleID == bundleID.Value);
            int pageSize = 50;
            int pageNumber = (page ?? 1);
            ViewBag.Bundles = db.Bundles.Where(x => x.Indicators.Where(y => y.Equations.Any()).Any()).ToDictionary(x=>x.ID,x=>x.Name);
            if (bundleID.HasValue)
            {
                ViewBag.BundleID = bundleID.Value;
            }
            return View(indicators.ToPagedList(pageNumber, pageSize));
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

            var bundles = db.Bundles.Include(x => x.Indicators).AsQueryable();
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
                Log(LogAction.Create, equation);
                return RedirectToAction("Index");
            }

            var indicator = db.Indicators.Find(equation.IndicatorID);
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
                Log(LogAction.Update, equation);
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
            Equation _equation = new Equation() { ID = id, Indicator = equation.Indicator, EquationYears = equation.EquationYears };
            db.Equations.Remove(equation);
            db.SaveChanges();
            Log(LogAction.Delete, _equation);
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

        private IQueryable<Indicator> SortParams(string sortOrder, IQueryable<Indicator> indicators, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
                indicators = indicators.Where(x =>
                x.Code.ToString().Contains(searchString) ||
                x.Name.Contains(searchString) ||
                x.Bundle.Name.Contains(searchString)
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
                    indicators = indicators.OrderByDescending(s => s.Code);
                    break;
                case "IndicatorID":
                    indicators = indicators.OrderBy(s => s.Code);
                    break;
                case "IndicatorNameDesc":
                    indicators = indicators.OrderByDescending(s => s.Name);
                    break;
                case "IndicatorName":
                    indicators = indicators.OrderBy(s => s.Name);
                    break;               
                case "BundleNameDesc":
                    indicators = indicators.OrderByDescending(s => s.Bundle.Name);
                    break;
                case "BundleName":
                    indicators = indicators.OrderBy(s => s.Bundle.Name);
                    break;
                case "IDDesc":
                    indicators = indicators.OrderByDescending(s => s.ID);
                    break;
                default:
                    indicators = indicators.OrderBy(s => s.ID);
                    break;
            }
            return indicators;
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

