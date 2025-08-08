using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoDB2.Controllers
{
    [CheckNVSession]
    public class HomeNVController : Controller
    {
        // GET: HomeNV
        public ActionResult TrangChuNV()
        {
            return View();
        }
    }
}