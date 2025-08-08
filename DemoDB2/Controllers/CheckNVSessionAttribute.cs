using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoDB2.Controllers
{
    public class CheckNVSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["NameUser"] == null)
            {
                filterContext.Result = new RedirectResult("~/LoginNhanVien/LoginNV");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}