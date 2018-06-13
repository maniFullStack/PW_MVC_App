using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace PWPortal.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Bundling scripts
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                      "~/Scripts/jquery-3.2.1.js",
                      "~/Scripts/modernizr-2.6.2.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/bootstrap.js",   
                      "~/Scripts/jquery.unobtrusive-ajax.js",
                      "~/Scripts/jquery.unobtrusive-ajax.min.js",
                      "~/Scripts/datatables.js",
                      "~/Scripts/PWScript.js",
                      "~/Scripts/bootstrap-datepicker.js"
                      ));


            //Bundling styles
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                     "~/Content/bootstrap.css",
                     "~/Content/bootstrap.min.css",
                     "~/Content/Site.css",
                     "~/Content/datatables.css",
                     "~/Content/bootstrap-datepicker.css"
                     ));

            BundleTable.EnableOptimizations = true;
        }
    }
}