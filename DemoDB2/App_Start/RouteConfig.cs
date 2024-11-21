using System.Web.Mvc;
using System.Web.Routing;

namespace DemoDB2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //Trả về thanh toán
                routes.MapRoute(
        name: "ConfirmPayment",
        url: "HoaDon/ConfirmPayment",
        defaults: new { controller = "HoaDon", action = "ConfirmPayment" }
    );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
