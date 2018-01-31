using System.Web;
using System.Web.Optimization;

namespace EECIP
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //****************** JAVASCRIPT ***********************************
            //****************** JAVASCRIPT ***********************************
            //MSFT default
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Content/Scripts/bootstrap-hover-dropdown.js"));

            bundles.Add(new ScriptBundle("~/bundles/clip-main").Include("~/Scripts/clip-main.js"));
            bundles.Add(new ScriptBundle("~/bundles/confirmdelete").Include("~/Scripts/confirm_delete.js"));
            bundles.Add(new ScriptBundle("~/bundles/select2").Include("~/Scripts/select2.js"));
            bundles.Add(new ScriptBundle("~/bundles/toastr").Include("~/Scripts/toastr.js"));
            bundles.Add(new ScriptBundle("~/bundles/autocomplete").Include("~/Scripts/auto-complete.js"));
            bundles.Add(new ScriptBundle("~/bundles/anchorme").Include("~/Scripts/anchorme.js"));
            bundles.Add(new ScriptBundle("~/bundles/pagination").Include("~/Scripts/jquery.twbsPagination.js"));
            bundles.Add(new ScriptBundle("~/bundles/trunk8").Include("~/Scripts/trunk8.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstraptour").Include(
                "~/Scripts/bootstrap-tour.js",
                "~/Scripts/bootstrap-tour-dtl.js"));

            //****************** CSS ***********************************
            //****************** CSS ***********************************
            bundles.Add(new StyleBundle("~/Content/styles").Include(
                "~/Content/fonts.css",
                "~/Content/bootstrap.css",
                "~/fonts/clip-style.css",
                "~/Content/main.css",
                "~/Content/main-responsive.css",
                "~/Content/theme_dark.css",
                "~/Content/css/select2.css",
                "~/Content/auto-complete.css",
                "~/Content/bootstrap-tour.css",
                "~/Content/toastr.css",
                "~/Content/site.css"));
        }
    }
}
