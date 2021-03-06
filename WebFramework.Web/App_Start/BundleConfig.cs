﻿using System.Web.Optimization;

namespace Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            "~/Scripts/jquery-ui-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqgrid").Include(
                "~/Scripts/jquery.jqGrid.js"));
                //"~/Scripts/i18n/grid.locale-en.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-datepicker.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/log4javascript").Include(
                "~/Scripts/log4javascript.js",
                "~/Scripts/errorhandler.js"));
            bundles.Add(new ScriptBundle("~/bundles/afterload").Include(
            "~/Scripts/jstz.js",
            "~/Scripts/jquery.cookie.js",
            "~/Scripts/Common.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-theme.css",
                      //"~/Content/font-awesome.css",
                      "~/Content/site.css"));
            //bundles.Add(new StyleBundle("~/Content/jquery").Include(
            //    "~/Content/themes/redmond/jquery-ui.css"));
            bundles.Add(new StyleBundle("~/Content/jqgrid").Include(
                "~/Content/jquery.jqGrid/ui.jqgrid.css"));
                //"~/Content/jquery.jqGrid/ui.jqgrid-bootstrap.css"));
        }
    }
}
