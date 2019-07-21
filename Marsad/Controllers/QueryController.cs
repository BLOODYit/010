using Marsad.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marsad.Controllers
{
    public class QueryController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Query
        public ActionResult Indicators(string keywords, int[] bundleIds, int[] indicatorGroupIds)
        {
            ViewBag.Bundles = db.Bundles.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.IndicatorGroups = db.IndicatorGroups.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.keywords = keywords;
            ViewBag.bundleIds = bundleIds;
            ViewBag.indicatorGroupIds = indicatorGroupIds;

            var indicators = db.Indicators.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                indicators = indicators.Where(x => x.Name.Contains(keywords.Trim()) || x.Description.Contains(keywords.Trim()));
            }
            if (bundleIds != null && bundleIds.Length > 0)
            {
                indicators = indicators.Where(x => bundleIds.Contains(x.BundleID));
            }
            if (indicatorGroupIds != null && indicatorGroupIds.Length > 0)
            {
                indicators = indicators.Where(x => x.IndicatorGroups.Count(y => indicatorGroupIds.Contains(y.ID)) > 0);
            }
            indicators = indicators.Include(x=>x.Bundle);
            return View(indicators.ToList());
        }

        public ActionResult DataSources(string keywords, int[] dataSourceTypeIds, int[] datasourceGroupIds)
        {
            ViewBag.DataSourceTypes = db.DataSourceTypes.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.DataSourceGroups = db.DataSourceGroups.ToDictionary(x => x.ID, x => x.Name);
            ViewBag.keywords = keywords;
            ViewBag.dataSourceTypeIds = dataSourceTypeIds;
            ViewBag.datasourceGroupIds = datasourceGroupIds;

            var dataSources = db.DataSources.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                dataSources = dataSources.Where(x => x.Name.Contains(keywords.Trim()));
            }
            if (dataSourceTypeIds != null && dataSourceTypeIds.Length > 0)
            {
                dataSources = dataSources.Where(x => dataSourceTypeIds.Contains(x.DataSourceTypeID));
            }
            if (datasourceGroupIds != null && datasourceGroupIds.Length > 0)
            {
                dataSources = dataSources.Where(x => x.DataSourceGroups.Count(y => datasourceGroupIds.Contains(y.ID)) > 0);
            }
            
            return View(dataSources.ToList());            
        }

        public ActionResult DataSourceDetails(int[] ids)
        {
            var dataSources = db.DataSources.Where(x=>ids.Contains(x.ID));
            return View(dataSources);
        }

        public ActionResult Cases(string keywords, int[] indicatorIds, int[] yearIds)
        {
            ViewBag.Indicators = db.Indicators.Where(x=>x.CaseYearIndicators.Count>0).ToDictionary(x => x.ID, x => x.Name);
            ViewBag.Years = db.Cases.Select(x=>x.Year).Distinct().ToDictionary(x => x, x => x.ToString());
            ViewBag.keywords = keywords;
            ViewBag.indicatorIds = indicatorIds;
            ViewBag.yearIds = yearIds;

            var cases = db.Cases.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                cases = cases.Where(x => x.Name.Contains(keywords.Trim()) || x.Description.Contains(keywords.Trim()));
            }
            if (indicatorIds != null && indicatorIds.Length > 0)
            {
                var caseYearIds = db.CaseYearIndicators.Where(x=>indicatorIds.Contains(x.IndicatorID)).Select(x=>x.CaseYearID).Distinct().ToArray();
                var caseIds = db.CaseYears.Where(x => caseYearIds.Contains(x.ID)).Select(x => x.CaseID).Distinct().ToArray();
                cases = cases.Where(x => caseIds.Contains(x.ID));
            }
            if (yearIds != null && yearIds.Length > 0)
            {
                cases = cases.Where(x => yearIds.Contains(x.Year));
            }
            return View(cases.ToList());
        }

        public ActionResult CasesDetails(int[] ids)
        {
            var cases = db.Cases.Where(x => ids.Contains(x.ID));
            return View(cases);
        }
    }
}