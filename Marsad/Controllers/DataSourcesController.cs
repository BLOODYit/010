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
    public class DataSourcesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DataSources
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

            var dataSources = db.DataSources.Include(d => d.DataSourceType).Include(d => d.Entity).Include(d => d.Period).AsQueryable();
            dataSources = SortParams(sortOrder, dataSources, searchString);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(dataSources.ToPagedList(pageNumber, pageSize));
        }

        // GET: DataSources/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSource dataSource = db.DataSources.Find(id);
            if (dataSource == null)
            {
                return HttpNotFound();
            }
            return View(dataSource);
        }

        // GET: DataSources/Create
        public ActionResult Create()
        {
            ViewBag.DataSourceTypeID = new SelectList(db.DataSourceTypes, "ID", "Name");
            ViewBag.EntityID = new SelectList(db.Entities, "ID", "Name");
            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name");
            return View();
        }

        // POST: DataSources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,PublishDate,IsHijri,DataSourceTypeID,PublishNumber,PublisherName,AuthorName,IsPeriodic,PeriodID,HasEntity,EntityID,IsPart")] DataSource dataSource)
        {
            if (ModelState.IsValid)
            {
                var code = db.DataSources.Max(x => x.Code)+1;
                dataSource.Code = code;
                db.DataSources.Add(dataSource);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DataSourceTypeID = new SelectList(db.DataSourceTypes, "ID", "Name", dataSource.DataSourceTypeID);
            ViewBag.EntityID = new SelectList(db.Entities, "ID", "Name", dataSource.EntityID);
            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name", dataSource.PeriodID);
            return View(dataSource);
        }

        // GET: DataSources/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSource dataSource = db.DataSources.Find(id);
            if (dataSource == null)
            {
                return HttpNotFound();
            }
            ViewBag.DataSourceTypeID = new SelectList(db.DataSourceTypes, "ID", "Name", dataSource.DataSourceTypeID);
            ViewBag.EntityID = new SelectList(db.Entities, "ID", "Name", dataSource.EntityID);
            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name", dataSource.PeriodID);
            return View(dataSource);
        }

        // POST: DataSources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,PublishDate,IsHijri,DataSourceTypeID,PublishNumber,PublisherName,AuthorName,IsPeriodic,PeriodID,HasEntity,EntityID,IsPart")] DataSource dataSource)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dataSource).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DataSourceTypeID = new SelectList(db.DataSourceTypes, "ID", "Name", dataSource.DataSourceTypeID);
            ViewBag.EntityID = new SelectList(db.Entities, "ID", "Name", dataSource.EntityID);
            ViewBag.PeriodID = new SelectList(db.Periods, "ID", "Name", dataSource.PeriodID);
            return View(dataSource);
        }

        // GET: DataSources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSource dataSource = db.DataSources.Find(id);
            if (dataSource == null)
            {
                return HttpNotFound();
            }
            return View(dataSource);
        }

        // POST: DataSources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DataSource dataSource = db.DataSources.Find(id);
            db.DataSources.Remove(dataSource);
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

        private IQueryable<DataSource> SortParams(string sortOrder, IQueryable<DataSource> dataSources, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
                dataSources = dataSources.Where(x =>
                x.Name.Contains(searchString)
                || x.Code.ToString().Equals(searchString)
                || x.DataSourceType.Name.Contains(searchString)
                || x.Entity.Name.Contains(searchString));
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.CodeSortParm = sortOrder == "Code" ? "CodeDesc" : "Code";
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.PublishDateSortParm = sortOrder == "PublishDate" ? "PublishDateDesc" : "PublishDate";
            ViewBag.IsHijriSortParm = sortOrder == "IsHijri" ? "IsHijriDesc" : "IsHijri";
            ViewBag.DataSourceTypeIDSortParm = sortOrder == "DataSourceTypeID" ? "DataSourceTypeIDDesc" : "DataSourceTypeID";
            ViewBag.PublishNumberSortParm = sortOrder == "PublishNumber" ? "PublishNumberDesc" : "PublishNumber";
            ViewBag.PublisherNameSortParm = sortOrder == "PublisherName" ? "PublisherNameDesc" : "PublisherName";
            ViewBag.AuthorNameSortParm = sortOrder == "AuthorName" ? "AuthorNameDesc" : "AuthorName";
            ViewBag.IsPeriodicSortParm = sortOrder == "IsPeriodic" ? "IsPeriodicDesc" : "IsPeriodic";
            ViewBag.PeriodIDSortParm = sortOrder == "PeriodID" ? "PeriodIDDesc" : "PeriodID";
            ViewBag.HasEntitySortParm = sortOrder == "HasEntity" ? "HasEntityDesc" : "HasEntity";
            ViewBag.EntityIDSortParm = sortOrder == "EntityID" ? "EntityIDDesc" : "EntityID";
            ViewBag.IsPartSortParm = sortOrder == "IsPart" ? "IsPartDesc" : "IsPart";



            switch (sortOrder)
            {
                case "CodeDesc":
                    dataSources = dataSources.OrderByDescending(s => s.Code);
                    break;
                case "Code":
                    dataSources = dataSources.OrderBy(s => s.Code);
                    break;
                case "NameDesc":
                    dataSources = dataSources.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    dataSources = dataSources.OrderBy(s => s.Name);
                    break;
                case "PublishDateDesc":
                    dataSources = dataSources.OrderByDescending(s => s.PublishDate);
                    break;
                case "PublishDate":
                    dataSources = dataSources.OrderBy(s => s.PublishDate);
                    break;
                case "IsHijriDesc":
                    dataSources = dataSources.OrderByDescending(s => s.IsHijri);
                    break;
                case "IsHijri":
                    dataSources = dataSources.OrderBy(s => s.IsHijri);
                    break;
                case "DataSourceTypeIDDesc":
                    dataSources = dataSources.OrderByDescending(s => s.DataSourceType.Name);
                    break;
                case "DataSourceTypeID":
                    dataSources = dataSources.OrderBy(s => s.DataSourceType.Name);
                    break;
                case "PublishNumberDesc":
                    dataSources = dataSources.OrderByDescending(s => s.PublishNumber);
                    break;
                case "PublishNumber":
                    dataSources = dataSources.OrderBy(s => s.PublishNumber);
                    break;
                case "PublisherNameDesc":
                    dataSources = dataSources.OrderByDescending(s => s.PublisherName);
                    break;
                case "PublisherName":
                    dataSources = dataSources.OrderBy(s => s.PublisherName);
                    break;
                case "AuthorNameDesc":
                    dataSources = dataSources.OrderByDescending(s => s.AuthorName);
                    break;
                case "AuthorName":
                    dataSources = dataSources.OrderBy(s => s.AuthorName);
                    break;
                case "IsPeriodicDesc":
                    dataSources = dataSources.OrderByDescending(s => s.IsPeriodic);
                    break;
                case "IsPeriodic":
                    dataSources = dataSources.OrderBy(s => s.IsPeriodic);
                    break;
                case "PeriodIDDesc":
                    dataSources = dataSources.OrderByDescending(s => s.PeriodID);
                    break;
                case "PeriodID":
                    dataSources = dataSources.OrderBy(s => s.PeriodID);
                    break;
                case "HasEntityDesc":
                    dataSources = dataSources.OrderByDescending(s => s.HasEntity);
                    break;
                case "HasEntity":
                    dataSources = dataSources.OrderBy(s => s.HasEntity);
                    break;
                case "EntityIDDesc":
                    dataSources = dataSources.OrderByDescending(s => s.Entity.Name);
                    break;
                case "EntityID":
                    dataSources = dataSources.OrderBy(s => s.Entity.Name);
                    break;
                case "IsPartDesc":
                    dataSources = dataSources.OrderByDescending(s => s.IsPart);
                    break;
                case "IsPart":
                    dataSources = dataSources.OrderBy(s => s.IsPart);
                    break;

                case "IDDesc":
                    dataSources = dataSources.OrderByDescending(s => s.ID);
                    break;
                default:
                    dataSources = dataSources.OrderBy(s => s.ID);
                    break;
            }
            return dataSources;
        }

    }


}

