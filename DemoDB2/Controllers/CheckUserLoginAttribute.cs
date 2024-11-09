//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace DemoDB2.Controllers
//{
//    public class CheckUserLoginAttribute : ActionFilterAttribute
//    {
//        public override void OnActionExecuting(ActionExecutingContext filterContext)
//        {
//            if (filterContext.HttpContext.Session["NguoiDungID"] == null)
//            {
//                filterContext.Result = new RedirectResult("~/LoginUser/Login");
//                return;
//            }
//            base.OnActionExecuting(filterContext);
//        }
//    }
//}