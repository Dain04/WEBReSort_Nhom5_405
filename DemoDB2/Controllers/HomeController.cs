using DemoDB2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoDB2.Controllers
{
    public class HomeController : Controller
    {
        QLKSEntities database = new QLKSEntities();
        // GET: TrangChu
        public ActionResult TrangChu()
        {
            return View();
        }
        public ActionResult DetailPhongVip()
        {
            var data = database.Phong.ToList();
            return View(data);
        }

        public ActionResult DetailPhongThuong()
        {
            var data = database.Phong.ToList();
            return View(data);
        }
        public ActionResult XacNhanDatPhong()
        {
            return View();
        }
        public ActionResult AboutUs()
        {
            var data = database.Phong.ToList();
            return View(data);
        }
    }
}