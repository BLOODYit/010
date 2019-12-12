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


            int pageSize = 10;
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

            int pageSize = 10;
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

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var result = db.ElementValues
                .Include(x => x.GeoArea)
                .Include(x => x.EquationYear.Equation.Indicator)
                .Where(x => x.IsCommited == false)
                .GroupBy(x => new { x.GeoArea, x.EquationYear })
                .Select(x => new PendingLog()
                {
                    ActionDate = x.Max(y => y.CreatedAt),
                    UserName = x.Max(y => y.ApplicationUser.Name),
                    Log = "بيان مؤشر " + x.Key.EquationYear.Equation.Indicator.Name + " الخاص بـ" + x.Key.GeoArea.Name + " لسنة " + x.Key.EquationYear.Year.ToString(),
                    EquationYearID=x.Key.EquationYear.ID,
                    GeoAreaID=x.Key.GeoArea.ID
                })
                .OrderBy(x => x.ActionDate)
                .ToPagedList(pageNumber, pageSize);

            return View(result);
        }
    }
}