using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoDB2.Controllers
{
    public class CheckUserLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session != null)
            {
                if (!SessionManager.IsUserLoggedIn() && !SessionManager.IsNhanVienLoggedIn())
                {
                    filterContext.Result = new RedirectResult("~/LoginUser/Index");
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}