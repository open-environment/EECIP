﻿using System.Web;
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
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

            //EECIP custom
            bundles.Add(new ScriptBundle("~/bundles/clip-main").Include("~/Scripts/clip-main.js"));
            bundles.Add(new ScriptBundle("~/bundles/confirmdelete").Include("~/Scripts/confirm_delete.js"));
            bundles.Add(new ScriptBundle("~/bundles/select2").Include("~/Scripts/select2.js"));
            bundles.Add(new ScriptBundle("~/bundles/toastr").Include("~/Scripts/toastr.js"));
            bundles.Add(new ScriptBundle("~/bundles/autocomplete").Include("~/Scripts/auto-complete.js"));
            bundles.Add(new ScriptBundle("~/bundles/anchorme").Include("~/Scripts/anchorme.js"));
            bundles.Add(new ScriptBundle("~/bundles/pagination").Include("~/Scripts/jquery.twbsPagination.js"));
            bundles.Add(new ScriptBundle("~/bundles/stickytableheaders").Include("~/Scripts/jquery.stickytableheaders.js"));
            bundles.Add(new ScriptBundle("~/bundles/trunk8").Include("~/Scripts/trunk8.js"));
            bundles.Add(new ScriptBundle("~/bundles/areyousure").Include("~/Scripts/jquery.are-you-sure.js"));
            bundles.Add(new ScriptBundle("~/bundles/chart").Include("~/Scripts/Chart.bundle.js"));
            bundles.Add(new ScriptBundle("~/bundles/introjs").Include("~/Scripts/intro.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                "~/Scripts/dataTables/jquery.dataTables.min.js",
                "~/Scripts/dataTables/dataTables.bootstrap.min.js",
                "~/Scripts/dataTables/plugins/dataTables.select.min.js",
                "~/Scripts/dataTables/plugins/moment.js",
                "~/Scripts/dataTables/plugins/datetime.js"));


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
                "~/Content/toastr.css",
                "~/Content/introjs.min.css",
                "~/Content/site.css",
                "~/Scripts/dataTables/plugins/select.bootstrap.min.css",
                "~/Scripts/dataTables/dataTables.bootstrap.min.css"
                ));
        }
    }
}
