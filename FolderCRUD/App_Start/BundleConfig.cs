using System.Web;
using System.Web.Optimization;

namespace FolderManager {
    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Angular-ui-grid
            bundles.Add(new StyleBundle("~/Content/ui-grid").Include(
                        "~/Content/ui-grid.css"));

            bundles.Add(new ScriptBundle("~/bundles/ui-grid").Include(
                        "~/Scripts/ui-grid.js"));

            // Select ui angular
            bundles.Add(new StyleBundle("~/Content/ui-select").Include(
                        "~/Content/select.css"));

            bundles.Add(new ScriptBundle("~/bundles/ui-select").Include(
                        "~/Scripts/select.js"));

            //AngularJS
            bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
                        "~/Scripts/angular.js",
                        "~/Scripts/angular-table.min.js",
                        "~/Scripts/angular-sanitize.min.js",
                        "~/Scripts/Angular/App.js"));

            // FeriadoModule
            bundles.Add(new ScriptBundle("~/bundles/FolderModule").Include(
                        "~/Scripts/Angular/Modules/FolderManagerModule.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/site.css"));
            // Alertify
            bundles.Add(new ScriptBundle("~/bundles/Alertify").Include(
                        "~/Scripts/alertify.js"));

            bundles.Add(new StyleBundle("~/Content/Alertify").Include(
                        "~/Content/alertifyjs/alertify.min.css", "~/Content/alertifyjs/themes/bootstrap.min.css"));

            //AdminLTE
            bundles.Add(new StyleBundle("~/Content/AdminLTE").Include(
                        "~/Content/AdminLTE/AdminLTE.css",
                        "~/Content/AdminLTE/skin-black-light.css",
                        "~/Content/AdminLTE/skin-blue.css",
                        "~/Content/AdminLTE/skin-sishco.css",
                        "~/Content/AdminLTE/dataTables.bootstrap.min.css",
                        "~/Content/custom.css"));

            bundles.Add(new ScriptBundle("~/bundles/AdminLTE").Include(
                        "~/Scripts/AdminLTE/jquery.dataTables.min.js",
                        "~/Scripts/AdminLTE/dataTables.bootstrap.min.js",
                        "~/Scripts/AdminLTE/adminlte.min.js"));

            //Font-Awesome
            bundles.Add(new StyleBundle("~/Content/font-awesome").Include(
                        "~/Content/font-awesome.min.css",
                        "~/Content/brand.css",
                        "~/Content/solid.css",
                        "~/Content/regular.css",
                        "~/Content/all.css",
                        "~/Content/v4-shims.css"));

            //jsTree
            bundles.Add(new StyleBundle("~/Content/treeview").Include(
                     "~/Content/jsTree3/themes/proton/style.min.css",
                     "~/Content/jsTree3/themes/default/style.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/treeview").Include(
               "~/Scripts/jsTree3/jstree.min.js"));
        }
    }
}
