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
    public class DataSourceGroupsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DataSourceGroups
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

            var dataSourceGroups = db.DataSourceGroups.AsQueryable();
            dataSourceGroups = SortParams(sortOrder, dataSourceGroups, searchString);
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(dataSourceGroups.ToPagedList(pageNumber, pageSize));
        }

        // GET: DataSourceGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceGroup dataSourceGroup = db.DataSourceGroups.Find(id);
            if (dataSourceGroup == null)
            {
                return HttpNotFound();
            }
            return View(dataSourceGroup);
        }

        // GET: DataSourceGroups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DataSourceGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Name")] DataSourceGroup dataSourceGroup)
        {
            if (ModelState.IsValid)
            {
                db.DataSourceGroups.Add(dataSourceGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dataSourceGroup);
        }

        // GET: DataSourceGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceGroup dataSourceGroup = db.DataSourceGroups.Find(id);
            if (dataSourceGroup == null)
            {
                return HttpNotFound();
            }
            return View(dataSourceGroup);
        }

        // POST: DataSourceGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Name")] DataSourceGroup dataSourceGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dataSourceGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dataSourceGroup);
        }

        // GET: DataSourceGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceGroup dataSourceGroup = db.DataSourceGroups.Find(id);
            if (dataSourceGroup == null)
            {
                return HttpNotFound();
            }
            return View(dataSourceGroup);
        }

        // POST: DataSourceGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DataSourceGroup dataSourceGroup = db.DataSourceGroups.Find(id);
            db.DataSourceGroups.Remove(dataSourceGroup);
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

        private IQueryable<DataSourceGroup> SortParams(string sortOrder, IQueryable<DataSourceGroup> dataSourceGroups, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
                dataSourceGroups = dataSourceGroups.Where(x => x.Name.Contains(searchString));
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.CodeSortParm = sortOrder == "Code" ? "CodeDesc" : "Code";
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";



            switch (sortOrder)
            {
                case "CodeDesc":
                    dataSourceGroups = dataSourceGroups.OrderByDescending(s => s.Code);
                    break;
                case "Code":
                    dataSourceGroups = dataSourceGroups.OrderBy(s => s.Code);
                    break;
                case "NameDesc":
                    dataSourceGroups = dataSourceGroups.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    dataSourceGroups = dataSourceGroups.OrderBy(s => s.Name);
                    break;

                case "IDDesc":
                    dataSourceGroups = dataSourceGroups.OrderByDescending(s => s.ID);
                    break;
                default:
                    dataSourceGroups = dataSourceGroups.OrderBy(s => s.ID);
                    break;
            }
            return dataSourceGroups;
        }

    }


}

