using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Marsad.Helpers
{
    public static class Extensions
    {
        public static string IsSelected(this HtmlHelper html, string controllers = "", string actions = "", string cssClass = "active")
        {
            ViewContext viewContext = html.ViewContext;
            bool isChildAction = viewContext.Controller.ControllerContext.IsChildAction;

            if (isChildAction)
                viewContext = html.ViewContext.ParentActionViewContext;

            RouteValueDictionary routeValues = viewContext.RouteData.Values;
            string currentAction = routeValues["action"].ToString();
            string currentController = routeValues["controller"].ToString();

            if (String.IsNullOrEmpty(actions))
                actions = currentAction;

            if (String.IsNullOrEmpty(controllers))
                controllers = currentController;

            string[] acceptedActions = actions.Trim().Split(',').Distinct().ToArray();
            string[] acceptedControllers = controllers.Trim().Split(',').Distinct().ToArray();

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ?
                cssClass : String.Empty;
        }

        public static string GetSortingClass(this HtmlHelper html,object currentSort,string column)
        {
            if (currentSort == null)
                return "fa fa-sort";
            if (currentSort.ToString().Equals(column, StringComparison.CurrentCultureIgnoreCase))
                return "fa fa-sort-amount-asc";
            else if (currentSort.ToString().Equals(column+"Desc", StringComparison.CurrentCultureIgnoreCase))
                return "fa fa-sort-amount-desc";
            return "fa fa-sort";

        }
    }
}