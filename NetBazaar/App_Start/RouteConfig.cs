using System.Web.Mvc;
using System.Web.Routing;

namespace NetBazaar
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                );

            routes.MapRoute(
              "ShowCategories",
              "Postings/Categories",
              new { controller = "Postings", action = "Index" }
              );

            routes.MapRoute(
                "ShowCategoryPostings",
                "Postings/Category/{id}",
                new {controller = "Postings", action = "CategoryPostings", id = UrlParameter.Optional}
                );

            routes.MapRoute(
                "CreateCategoryPosting",
                "Postings/Category/{id}/Create",
                new {controller = "Postings", action = "Create", id = UrlParameter.Optional}
                );
        }
    }
}