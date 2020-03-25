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
    [Authorize(Roles = "Admin,Officer,Visitor")]
    public class IndicatorsController : BaseController
    {

        // GET: Indicators
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? pageSize,string indicatorType)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.indicatorType = indicatorType;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var indicators = db.Indicators.Include(i => i.Bundle).Include(i => i.IndicatorType).Include(i => i.ParentIndicator).AsQueryable();
            indicators = SortParams(sortOrder, indicators, searchString);
            if (!string.IsNullOrEmpty(indicatorType))
            {
                if (indicatorType.Equals("parent"))
                {
                    indicators = indicators.Where(x => !x.IndicatorID.HasValue);
                }
                else if (indicatorType.Equals("child"))
                {
                    indicators = indicators.Where(x => x.IndicatorID.HasValue);
                }
            }
            if (!pageSize.HasValue)
                pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.PageSize = new List<SelectListItem>()
         {
             new SelectListItem() { Value="10", Text= "10",Selected=(pageSize==10)},
             new SelectListItem() { Value="20", Text= "20",Selected=(pageSize==20) },
             new SelectListItem() { Value="50", Text= "50" ,Selected=(pageSize==50)},
             new SelectListItem() { Value="100", Text= "100" ,Selected=(pageSize==100)},
             new SelectListItem() { Value="200", Text= "200" ,Selected=(pageSize==200)},
             new SelectListItem() { Value="500", Text= "500" ,Selected=(pageSize==500)},
             new SelectListItem() { Value="1000", Text= "1000",Selected=(pageSize==1000) }
         };

            ViewBag.pSize = pageSize.Value;

            return View(indicators.ToPagedList(pageNumber, pageSize.Value));
        }

        // GET: Indicators/Details/5
        [Authorize(Roles = "Admin,Officer,Visitor")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Indicator indicator = db.Indicators.Include(x => x.ChildIndicators).Where(x => x.ID == id).FirstOrDefault();
            if (indicator == null)
            {
                return HttpNotFound();
            }
            return View(indicator);
        }

        // GET: Indicators/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.BundleID = new SelectList(db.Bundles, "ID", "Name");
            ViewBag.IndicatorTypeID = new SelectList(db.IndicatorTypes, "ID", "Name");
            ViewBag.IndicatorID = new SelectList(db.Indicators.Where(x => !x.HasParent), "ID", "Name");
            var MaxCode = db.Indicators.Max(x => (int?)x.Code);
            ViewBag.NextCode = (MaxCode.HasValue) ? MaxCode.Value + 1 : 1;
            return View();
        }

        // POST: Indicators/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Name,MeasureUnit,HasParent,IndicatorID,IndicatorTypeID,BundleID,Description,Correlation,GeoArea,References,CalculationMethod,ElementCount,IndicatorImportance")] Indicator indicator, string command)
        {
            if (ModelState.IsValid)
            {
                db.Indicators.Add(indicator);
                db.SaveChanges();
                Log(LogAction.Create, indicator);
                if (command.Equals("إنشاء"))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create", "Equations", new { indicatorID = indicator.ID });
            }

            ViewBag.BundleID = new SelectList(db.Bundles, "ID", "Name", indicator.BundleID);
            ViewBag.IndicatorTypeID = new SelectList(db.IndicatorTypes, "ID", "Name", indicator.IndicatorTypeID);
            ViewBag.IndicatorID = new SelectList(db.Indicators.Where(x => !x.HasParent), "ID", "Name", indicator.IndicatorID);
            return View(indicator);
        }

        // GET: Indicators/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicator indicator = db.Indicators.Find(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            ViewBag.BundleID = new SelectList(db.Bundles, "ID", "Name", indicator.BundleID);
            ViewBag.IndicatorTypeID = new SelectList(db.IndicatorTypes, "ID", "Name", indicator.IndicatorTypeID);
            ViewBag.IndicatorID = new SelectList(db.Indicators.Where(x => !x.HasParent), "ID", "Name", indicator.IndicatorID);
            return View(indicator);
        }

        // POST: Indicators/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Name,MeasureUnit,HasParent,IndicatorID,IndicatorTypeID,BundleID,Description,Correlation,GeoArea,References,CalculationMethod,ElementCount,IndicatorImportance")] Indicator indicator)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indicator).State = EntityState.Modified;
                db.SaveChanges();
                Log(LogAction.Update, indicator);
                return RedirectToAction("Index");
            }
            ViewBag.BundleID = new SelectList(db.Bundles, "ID", "Name", indicator.BundleID);
            ViewBag.IndicatorTypeID = new SelectList(db.IndicatorTypes, "ID", "Name", indicator.IndicatorTypeID);
            ViewBag.IndicatorID = new SelectList(db.Indicators.Where(x => !x.HasParent), "ID", "Name", indicator.IndicatorID);
            return View(indicator);
        }

        // GET: Indicators/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicator indicator = db.Indicators.Find(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            return View(indicator);
        }

        // POST: Indicators/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Indicator indicator = db.Indicators.Find(id);
            Indicator _indicator = new Indicator() { ID = id, Name = indicator.Name };
            db.Indicators.Remove(indicator);
            db.SaveChanges();
            Log(LogAction.Delete, _indicator);
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
                indicators = indicators.Where(x => x.Name.Contains(searchString) || x.Code.ToString().Contains(searchString) || (x.Bundle != null && x.Bundle.Name.Contains(searchString)));


            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.CodeSortParm = sortOrder == "Code" ? "CodeDesc" : "Code";
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.MeasureUnitSortParm = sortOrder == "MeasureUnit" ? "MeasureUnitDesc" : "MeasureUnit";
            ViewBag.HasParentSortParm = sortOrder == "HasParent" ? "HasParentDesc" : "HasParent";
            ViewBag.IndicatorIDSortParm = sortOrder == "IndicatorID" ? "IndicatorIDDesc" : "IndicatorID";
            ViewBag.IndicatorTypeIDSortParm = sortOrder == "IndicatorTypeID" ? "IndicatorTypeIDDesc" : "IndicatorTypeID";
            ViewBag.BundleIDSortParm = sortOrder == "BundleID" ? "BundleIDDesc" : "BundleID";
            ViewBag.DescriptionSortParm = sortOrder == "Description" ? "DescriptionDesc" : "Description";
            ViewBag.CorrelationSortParm = sortOrder == "Correlation" ? "CorrelationDesc" : "Correlation";
            ViewBag.GeoAreaSortParm = sortOrder == "GeoArea" ? "GeoAreaDesc" : "GeoArea";
            ViewBag.ReferencesSortParm = sortOrder == "References" ? "ReferencesDesc" : "References";
            ViewBag.CalculationMethodSortParm = sortOrder == "CalculationMethod" ? "CalculationMethodDesc" : "CalculationMethod";



            switch (sortOrder)
            {
                case "CodeDesc":
                    indicators = indicators.OrderByDescending(s => s.Code);
                    break;
                case "Code":
                    indicators = indicators.OrderBy(s => s.Code);
                    break;
                case "NameDesc":
                    indicators = indicators.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    indicators = indicators.OrderBy(s => s.Name);
                    break;
                case "MeasureUnitDesc":
                    indicators = indicators.OrderByDescending(s => s.MeasureUnit);
                    break;
                case "MeasureUnit":
                    indicators = indicators.OrderBy(s => s.MeasureUnit);
                    break;
                case "HasParentDesc":
                    indicators = indicators.OrderByDescending(s => s.HasParent);
                    break;
                case "HasParent":
                    indicators = indicators.OrderBy(s => s.HasParent);
                    break;
                case "IndicatorIDDesc":
                    indicators = indicators.OrderByDescending(s => s.ParentIndicator.Name);
                    break;
                case "IndicatorID":
                    indicators = indicators.OrderBy(s => s.ParentIndicator.Name);
                    break;
                case "IndicatorTypeIDDesc":
                    indicators = indicators.OrderByDescending(s => s.IndicatorType.Name);
                    break;
                case "IndicatorTypeID":
                    indicators = indicators.OrderBy(s => s.IndicatorType.Name);
                    break;
                case "BundleIDDesc":
                    indicators = indicators.OrderByDescending(s => s.Bundle.Name);
                    break;
                case "BundleID":
                    indicators = indicators.OrderBy(s => s.Bundle.Name);
                    break;
                case "DescriptionDesc":
                    indicators = indicators.OrderByDescending(s => s.Description);
                    break;
                case "Description":
                    indicators = indicators.OrderBy(s => s.Description);
                    break;
                case "CorrelationDesc":
                    indicators = indicators.OrderByDescending(s => s.Correlation);
                    break;
                case "Correlation":
                    indicators = indicators.OrderBy(s => s.Correlation);
                    break;
                case "ReferencesDesc":
                    indicators = indicators.OrderByDescending(s => s.References);
                    break;
                case "References":
                    indicators = indicators.OrderBy(s => s.References);
                    break;
                case "CalculationMethodDesc":
                    indicators = indicators.OrderByDescending(s => s.CalculationMethod);
                    break;
                case "CalculationMethod":
                    indicators = indicators.OrderBy(s => s.CalculationMethod);
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

        [HttpGet]
        public ActionResult GetCreateIndicator()
        {
            //var indicators = db.Indicators.Where(x => !x.HasParent);
            ViewBag.BundleID = new SelectList(db.Bundles, "ID", "Name");
            ViewBag.IndicatorTypeID = new SelectList(db.IndicatorTypes, "ID", "Name");
            var MaxCode = db.Indicators.Max(x => (int?)x.Code);
            ViewBag.NextCode = (MaxCode.HasValue) ? MaxCode.Value + 1 : 1;
            return PartialView();
        }

        [HttpPost]
        public JsonResult PostCreateIndicator([Bind(Include = "ID,Code,Name,MeasureUnit,IndicatorTypeID,BundleID,Description,Correlation,GeoArea,References,CalculationMethod")] Indicator indicator)
        {
            if (ModelState.IsValid)
            {
                db.Indicators.Add(indicator);
                db.SaveChanges();
                Log(LogAction.Create, indicator);
                return Json(new { success = true, data = indicator });
            }
            else
            {
                return Json(new { success = false, data = indicator, errors = ModelState.Values.Where(i => i.Errors.Count > 0) });
            }
        }

        [HttpGet]
        public JsonResult IsExist(int Code, int? ID)
        {
            bool isExists = false;
            if (!ID.HasValue)
                isExists = db.Indicators.Where(x => x.Code == Code).Any();
            else
                isExists = db.Indicators.Where(x => x.Code == Code && x.ID != ID.Value).Any();
            return Json(!isExists, JsonRequestBehavior.AllowGet);
        }
    }
}