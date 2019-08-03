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
    public class IndicatorGroupsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: IndicatorGroups
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

            var indicatorGroups = db.IndicatorGroups.AsQueryable();
            indicatorGroups = SortParams(sortOrder, indicatorGroups, searchString);
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(indicatorGroups.ToPagedList(pageNumber, pageSize));
        }

        // GET: IndicatorGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorGroup indicatorGroup = db.IndicatorGroups.Find(id);
            if (indicatorGroup == null)
            {
                return HttpNotFound();
            }
            return View(indicatorGroup);
        }

        // GET: IndicatorGroups/Create
        public ActionResult Create()
        {
            ViewBag.Indicators = db.Indicators.ToDictionary(x=>x.ID,x=>x.Name);
            return View();
        }

        // POST: IndicatorGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Name,Description")] IndicatorGroup indicatorGroup,int[] indicatorIds)
        {
            if (ModelState.IsValid)
            {
                var indicators = db.Indicators.Where(x => indicatorIds.Contains(x.ID));
                foreach(var i in indicators)
                {
                    indicatorGroup.Indicators.Add(i);
                }
                
                db.IndicatorGroups.Add(indicatorGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Indicators = db.Indicators.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.indicatorIds = indicatorIds;            
            return View(indicatorGroup);
        }

        // GET: IndicatorGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorGroup indicatorGroup = db.IndicatorGroups.Include(x=>x.Indicators).Where(x=>x.ID==id).FirstOrDefault();
            if (indicatorGroup == null)
            {
                return HttpNotFound();
            }
            ViewBag.Indicators = db.Indicators.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.indicatorIds = indicatorGroup.Indicators.Select(x => x.ID).ToArray();
            return View(indicatorGroup);
        }

        // POST: IndicatorGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Name,Description")] IndicatorGroup indicatorGroup, int[] indicatorIds)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indicatorGroup).State = EntityState.Modified;
                var ig = db.IndicatorGroups.Include(x => x.Indicators).Where(x => x.ID == indicatorGroup.ID).FirstOrDefault();
                indicatorGroup.Indicators = ig.Indicators;
                indicatorGroup.Indicators.Clear();
                var indicators = db.Indicators.Where(x => indicatorIds.Contains(x.ID));
                foreach (var i in indicators)
                {
                    indicatorGroup.Indicators.Add(i);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Indicators = db.Indicators.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.indicatorIds = indicatorIds;            
            return View(indicatorGroup);
        }

        // GET: IndicatorGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorGroup indicatorGroup = db.IndicatorGroups.Find(id);
            if (indicatorGroup == null)
            {
                return HttpNotFound();
            }
            return View(indicatorGroup);
        }

        // POST: IndicatorGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            IndicatorGroup indicatorGroup = db.IndicatorGroups.Find(id);
            db.IndicatorGroups.Remove(indicatorGroup);
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

        private IQueryable<IndicatorGroup> SortParams(string sortOrder, IQueryable<IndicatorGroup> indicatorGroups, string searchString)
        {
            if (!String.IsNullOrWhiteSpace(searchString))
                indicatorGroups = indicatorGroups.Where(x => x.Name.Contains(searchString) || x.Description.Contains(searchString));
            ViewBag.IDSortParm = String.IsNullOrEmpty(sortOrder) ? "IDDesc" : "";
            ViewBag.CodeSortParm = sortOrder == "Code" ? "CodeDesc" : "Code";
            ViewBag.NameSortParm = sortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.DescriptionSortParm = sortOrder == "Description" ? "DescriptionDesc" : "Description";



            switch (sortOrder)
            {
                case "CodeDesc":
                    indicatorGroups = indicatorGroups.OrderByDescending(s => s.Code);
                    break;
                case "Code":
                    indicatorGroups = indicatorGroups.OrderBy(s => s.Code);
                    break;
                case "NameDesc":
                    indicatorGroups = indicatorGroups.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    indicatorGroups = indicatorGroups.OrderBy(s => s.Name);
                    break;
                case "DescriptionDesc":
                    indicatorGroups = indicatorGroups.OrderByDescending(s => s.Description);
                    break;
                case "Description":
                    indicatorGroups = indicatorGroups.OrderBy(s => s.Description);
                    break;

                case "IDDesc":
                    indicatorGroups = indicatorGroups.OrderByDescending(s => s.ID);
                    break;
                default:
                    indicatorGroups = indicatorGroups.OrderBy(s => s.ID);
                    break;
            }
            return indicatorGroups;
        }

    }


}

