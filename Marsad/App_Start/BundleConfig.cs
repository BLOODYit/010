using System.Web;
using System.Web.Optimization;

namespace Marsad
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Css
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/vendors/font-awesome/css/font-awesome.min.css",
                      "~/Content/build/css/custom.min.css",
                      "~/Content/rtl.css",
                      "~/Content/bootstrap-datepicker3.min.css"));

            bundles.Add(new StyleBundle("~/Content/NProgress").Include(
                      "~/Content/vendors/nprogress/nprogress.css"));
            bundles.Add(new StyleBundle("~/Content/iCheck").Include(
                      "~/Content/vendors/iCheck/skins/flat/green.css"));
            bundles.Add(new StyleBundle("~/Content/bootstrap-progressbar").Include(
                      "~/Content/vendors/bootstrap-progressbar/css/bootstrap-progressbar-3.3.4.min.css"));
            bundles.Add(new StyleBundle("~/Content/JQVMap").Include(
                      "~/Content/vendors/jqvmap/dist/jqvmap.min.css"));
            bundles.Add(new StyleBundle("~/Content/daterangepicker").Include(
                      "~/Content/vendors/bootstrap-daterangepicker/daterangepicker.css"));

            bundles.Add(new StyleBundle("~/Content/mapReportStyle").Include(
                      "~/Content/mapReport/jquery-jvectormap-2.0.5.css"));

            bundles.Add(new ScriptBundle("~/Content/mapReport").Include(
                      "~/Content/mapReport/jquery-jvectormap-2.0.5.min.js"
                      , "~/Content/mapReport/govs.js"));
            //Scripts


            bundles.Add(new ScriptBundle("~/scripts/charts").Include(
                "~/Scripts/canvasjs.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*", "~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                          "~/Scripts/bootstrap.js"
                        , "~/Content/vendors/fastclick/lib/fastclick.js"
                        , "~/Content/build/js/custom.js"
                        , "~/Content/bootstrap-datepicker.min.js"
                        , "~/Scripts/jspdf.min.js"
                        , "~/Scripts/Amiri-Regular-normal.js"
                        , "~/Scripts/jspdf.plugin.autotable.min.js"
                        , "~/Scripts/html2canvas.min.js"
                        , "~/Content/vendors/devbridge-autocomplete/dist/jquery.autocomplete.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/Scripts/nprogress").Include(
                "~/Content/vendors/nprogress/nprogress.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Chart").Include(
                "~/Content/vendors/Chart.js/dist/Chart.min.js"));
            bundles.Add(new ScriptBundle("~/Scripts/gauge").Include(
                "~/Content/vendors/gauge.js/dist/gauge.min.js"));
            bundles.Add(new ScriptBundle("~/Scripts/bootstrap-progressbar").Include(
                "~/Content/vendors/bootstrap-progressbar/bootstrap-progressbar.min.js"));
            bundles.Add(new ScriptBundle("~/Scripts/iCheck").Include(
                "~/Content/vendors/iCheck/icheck.min.js"));
            bundles.Add(new ScriptBundle("~/Scripts/flot").Include(
                "~/Content/vendors/Flot/jquery.flot.js",
                "~/Content/vendors/Flot/jquery.flot.pie.js",
                "~/Content/vendors/Flot/jquery.flot.time.js",
                "~/Content/vendors/Flot/jquery.flot.stack.js",
                "~/Content/vendors/Flot/jquery.flot.resize.js",
                "~/Content/vendors/flot.orderbars/js/jquery.flot.orderBars.js",
                "~/Content/vendors/flot-spline/js/jquery.flot.spline.min.js",
                "~/Content/vendors/flot.curvedlines/curvedLines.js"));

            bundles.Add(new ScriptBundle("~/Scripts/DateJS").Include(
                "~/Content/vendors/DateJS/build/date.js",
                "~/Content/vendors/moment/min/moment.min.js",
                "~/Content/vendors/bootstrap-daterangepicker/daterangepicker.js"));

            bundles.Add(new ScriptBundle("~/Scripts/JQVMap").Include(
               "~/Content/vendors/jqvmap/dist/jquery.vmap.js",
               "~/Content/vendors/jqvmap/dist/maps/jquery.vmap.ksa.js",
               "~/Content/govs.js"));


        }
    }
}
