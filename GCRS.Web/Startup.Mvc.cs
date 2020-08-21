using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GCRS.Web
{
    public partial class Startup
    {
        public void ConfigureRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default1",
                url: "{Controller}/{action}/{id1}/{id2}",
                defaults: new { action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        public void ConfigureFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public void ConfigureBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/Scripts/jquery-1.10.2.min.js",
                        "~/node_modules/jquery/dist/jquery.min.js",
                        "~/node_modules/popper.js/dist/umd/popper.min.js",
                        "~/node_modules/bootstrap/dist/js/bootstrap.min.js",
                        "~/scripts/datatables/jquery.datatables.js",
                        "~/scripts/datatables/datatables.bootstrap4.js",
                        "~/scripts/typeahead.bundle.js",
                       "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js" ,
                        "~/wwwroot/app.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/node_modules/bootstrap/dist/css/bootstrap.css",
                      "~/node_modules/ol/ol.css",
                      "~/content/datatables/css/datatables.bootstrap4.css",
                      "~/Content/app.css"));
        }
    }
}