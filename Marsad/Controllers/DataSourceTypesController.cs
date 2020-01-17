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
    public class DataSourceTypesController :BaseController
    {
        
        // GET: DataSourceTypes
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

            var dataSourceTypes = db.DataSourceTypes.AsQueryable();
            dataSourceTypes = SortParams(sortOrder, dataSourceTypes, searchString);
            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return View(dataSourceTypes.ToPagedList(pageNumber, pageSize));
        }

        // GET: DataSourceTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceType dataSourceType = db.DataSourceTypes.Find(id);
            if (dataSourceType == null)
            {
                return HttpNotFound();
            }
            return View(dataSourceType);
        }

        // GET: DataSourceTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DataSourceTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] DataSourceType dataSourceType)
        {
            if (ModelState.IsValid)
            {
                db.DataSourceTypes.Add(dataSourceType);
                db.SaveChanges();
                Log(LogAction.Create, dataSourceType);
                return RedirectToAction("Index");
            }

            return View(dataSourceType);
        }

        // GET: DataSourceTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceType dataSourceType = db.DataSourceTypes.Find(id);
            if (dataSourceType == null)
            {
                return HttpNotFound();
            }
            return View(dataSourceType);
        }

        // POST: DataSourceTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] DataSourceType dataSourceType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dataSourceType).State = EntityState.Modified;
                db.SaveChanges();
                Log(LogAction.Update, dataSourceType);
                return RedirectToAction("Index");
            }
            return View(dataSourceType);
        }

        // GET: DataSourceTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceType dataSourceType = db.DataSourceTypes.Find(id);
            if (dataSourceType == null)
            {
                return HttpNotFound();
            }
            return View(dataSourceType);
        }

        // POST: DataSourceTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DataSourceType dataSourceType = db.DataSourceTypes.Find(id);
            DataSourceType _dataSourceType = new DataSourceType() { ID=id,Name=dataSourceType.Name};
            db.DataSourceTypes.Remove(dataSourceType);
            db.SaveChanges();
            Log(LogAction.Delete, _dataSourceType);
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

        private IQueryable<DataSourceType> SortParams(string sortOrder, IQueryable<DataSourceType> dataSourceTypes, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
                dataSourceTypes = dataSourceTypes.Where(x => x.Name.Contains(searchString));
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";            
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";



            switch (sortOrder)
            {                
                case "NameDesc":
                    dataSourceTypes = dataSourceTypes.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    dataSourceTypes = dataSourceTypes.OrderBy(s => s.Name);
                    break;

                case "IDDesc":
                    dataSourceTypes = dataSourceTypes.OrderByDescending(s => s.ID);
                    break;
                default:
                    dataSourceTypes = dataSourceTypes.OrderBy(s => s.ID);
                    break;
            }
            return dataSourceTypes;
        }

    }


}

