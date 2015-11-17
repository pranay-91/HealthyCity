using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
namespace Healthycity
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/admin/styles")
                .Include("~/content/styles/bootstrap.css")
                .Include("~/content/styles/admin.css"));

            bundles.Add(new StyleBundle("~/styles")
                .Include("~/content/styles/bootstrap.css")
                .Include("~/content/styles/site.css"));

            bundles.Add(new ScriptBundle("~/admin/Scripts/js")
                .Include("~/scripts/jquery-2.1.4.js")
                .Include("~/scripts/jquery-validate.js")
                .Include("~/scripts/jquery.validate.js")
                .Include("~/scripts/jquery.validate.unobstructive.js")
                .Include("~/scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/Scripts/js")
                .Include("~/scripts/jquery-2.1.4.js")
                .Include("~/scripts/jquery-validate.js")
                .Include("~/scripts/jquery.validate.js")
                .Include("~/scripts/jquery.validate.unobstructive.js")
                .Include("~/scripts/bootstrap.js"));
            //Turns off bundle optimisation
            //BundleTable.EnableOptimizations = false;
        }
    }
}