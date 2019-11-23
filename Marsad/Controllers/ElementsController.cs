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
    public class ElementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Elements
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

            var elements = db.Elements.Include(e => e.DataSource).AsQueryable();
            elements = SortParams(sortOrder, elements, searchString);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(elements.ToPagedList(pageNumber, pageSize));
        }

        // GET: Elements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Element element = db.Elements.Find(id);
            if (element == null)
            {
                return HttpNotFound();
            }
            return View(element);
        }

        // GET: Elements/Create
        public ActionResult Create()
        {
            ViewBag.DataSourceID = new SelectList(db.DataSources, "ID", "Name");
            return View();
        }

        // POST: Elements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Name,MeasureUnit,DataSourceID")] Element element)
        {
            if (ModelState.IsValid)
            {
                db.Elements.Add(element);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DataSourceID = new SelectList(db.DataSources, "ID", "Name", element.DataSourceID);
            return View(element);
        }

        // GET: Elements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Element element = db.Elements.Find(id);
            if (element == null)
            {
                return HttpNotFound();
            }
            ViewBag.DataSourceID = new SelectList(db.DataSources, "ID", "Name", element.DataSourceID);
            return View(element);
        }

        // POST: Elements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Name,MeasureUnit,DataSourceID")] Element element)
        {
            if (ModelState.IsValid)
            {
                db.Entry(element).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DataSourceID = new SelectList(db.DataSources, "ID", "Name", element.DataSourceID);
            return View(element);
        }

        // GET: Elements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Element element = db.Elements.Find(id);
            if (element == null)
            {
                return HttpNotFound();
            }
            return View(element);
        }

        // POST: Elements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Element element = db.Elements.Find(id);
            db.Elements.Remove(element);
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

        private IQueryable<Element> SortParams(string sortOrder, IQueryable<Element> elements, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
                elements = elements.Where(x => x.Name.Contains(searchString) || x.DataSource.Name.Contains(searchString));
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.CodeSortParm = sortOrder == "Code" ? "CodeDesc" : "Code";
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.MeasureUnitSortParm = sortOrder == "MeasureUnit" ? "MeasureUnitDesc" : "MeasureUnit";
            ViewBag.DataSourceIDSortParm = sortOrder == "DataSourceID" ? "DataSourceIDDesc" : "DataSourceID";



            switch (sortOrder)
            {
                case "CodeDesc":
                    elements = elements.OrderByDescending(s => s.Code);
                    break;
                case "Code":
                    elements = elements.OrderBy(s => s.Code);
                    break;
                case "NameDesc":
                    elements = elements.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    elements = elements.OrderBy(s => s.Name);
                    break;
                case "MeasureUnitDesc":
                    elements = elements.OrderByDescending(s => s.MeasureUnit);
                    break;
                case "MeasureUnit":
                    elements = elements.OrderBy(s => s.MeasureUnit);
                    break;
                case "DataSourceIDDesc":
                    elements = elements.OrderByDescending(s => s.DataSource.Name);
                    break;
                case "DataSourceID":
                    elements = elements.OrderBy(s => s.DataSource.Name);
                    break;

                case "IDDesc":
                    elements = elements.OrderByDescending(s => s.ID);
                    break;
                default:
                    elements = elements.OrderBy(s => s.ID);
                    break;
            }
            return elements;
        }

        [HttpGet]
        public ActionResult GetCreateElement()
        {
            ViewBag.DataSourceID = new SelectList(db.DataSources, "ID", "Name");
            return PartialView();
        }

        [HttpPost]
        public JsonResult PostCreateElement([Bind(Include = "ID,Code,Name,MeasureUnit,DataSourceID")] Element element)
        {
            if (ModelState.IsValid)
            {
                db.Elements.Add(element);
                db.SaveChanges();
                return Json(new { success = true, data = element });
            }
            else
            {
                return Json(new { success = false, data = element, errors = ModelState.Values.Where(i => i.Errors.Count > 0) });
            }
        }

    }


}

