using Marsad.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Marsad.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LogsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult SystemLog(string sortOrder, string currentFilter, string searchString, int? page)
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


            int pageSize = 50;
            int pageNumber = (page ?? 1);
            var systemLogs = db.SystemLogs.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
                systemLogs = systemLogs.Where(x => x.Details.Contains(searchString) || x.Log.Contains(searchString) || x.UserName.Contains(searchString));
            systemLogs = systemLogs.OrderByDescending(x => x.ActionDate);
            return View(systemLogs.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult UpdateLog(string sortOrder, string currentFilter, string searchString, int? page)
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

            int pageSize = 50;
            int pageNumber = (page ?? 1);
            var updateLogs = db.UpdateLogs.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
                updateLogs = updateLogs.Where(x => x.Details.Contains(searchString) || x.Log.Contains(searchString) || x.UserName.Contains(searchString));
            updateLogs = updateLogs.OrderByDescending(x => x.ActionDate);
            return View(updateLogs.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult PendingLog(string sortOrder, string currentFilter, string searchString, int? page)
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

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            var query = db.ElementYearValues.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchString))
                query = query.Where(x => x.Element.Name.Contains(searchString));
            var result = query.Include(x => x.ApplicationUser)
                .Include(x => x.Element)
                .Where(x => x.IsCommited == false)
                .GroupBy(x => new { x.CreatedAt, x.ApplicationUser, x.Year, x.GeoArea })
                .Select(x => new PendingLog()
                {
                    ActionDate = x.Key.CreatedAt,
                    UserName = x.Key.ApplicationUser.Name,
                    Entity = x.Key.ApplicationUser.Entity,
                    Log = "تحديث قيم نطاق (" + x.Key.GeoArea.Name + ")",
                    Year = x.Key.Year,
                    ApplicationUserID = x.Key.ApplicationUser.Id,
                    GeoAreaID = x.Key.GeoArea.ID
                })
                .OrderBy(x => x.ActionDate)
                .ToPagedList(pageNumber, pageSize);
            var details = db.ElementYearValues.Include(x => x.Element).Where(x => x.IsCommited == false);
            if (!string.IsNullOrWhiteSpace(searchString))
                details = details.Where(x => x.Element.Name.Contains(searchString));
            ViewBag.Details = details;
            return View(result);
        }
    }
}