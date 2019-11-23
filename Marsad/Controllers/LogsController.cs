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
            return View(db.SystemLogs.ToPagedList(pageNumber, pageSize));
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
            return View(db.UpdateLogs.ToPagedList(pageNumber, pageSize));
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
            var elementValues = db.ElementValues
                .Include(x => x.EquationElement.Equation.Indicator)
                .Include(x => x.GeoArea)
                .Include(x => x.GeoAreaBundle)
                .Where(x => x.IsCommited == false);
            elementValues = elementValues.OrderByDescending(x => x.CreatedAt);
            return View(elementValues.ToPagedList(pageNumber, pageSize));
        }

    }
}