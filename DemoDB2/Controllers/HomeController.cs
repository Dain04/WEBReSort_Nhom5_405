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
        public ActionResult DatPhongVIP()
        {
            var phongVIP = database.Phong.Where(p => p.LoaiP == "VIP" && p.TinhTrang == true).ToList();
            return View(phongVIP);
        }

        [HttpPost]
        public ActionResult DatPhongVIP(int PhongID, DateTime NgayNhanPhong, DateTime NgayTraPhong)
        {
            if (ModelState.IsValid)
            {
                var datPhong = new DatPhong
                {
                    PhongID = PhongID,
                    NgayDatPhong = DateTime.Now,
                    NgayNhanPhong = NgayNhanPhong,
                    NgayTraPhong = NgayTraPhong,
                    NguoiDungID = 1 // Giả sử người dùng có ID là 1, cần thay đổi logic này để lấy ID người dùng thực tế
                };

                database.DatPhong.Add(datPhong);
                database.SaveChanges();

                return RedirectToAction("XacNhanDatPhong");
            }

            var phongVIP = database.Phong.Where(p => p.LoaiP == "VIP" && p.TinhTrang == true).ToList();
            return View(phongVIP);
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