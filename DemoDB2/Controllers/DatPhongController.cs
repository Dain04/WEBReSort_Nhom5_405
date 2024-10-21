using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DemoDB2.Models;
using Microsoft.AspNet.Identity;

namespace DemoDB2.Controllers
{
    [Authorize]
    public class DatPhongController : Controller
    {
        private QLKSEntities db = new QLKSEntities();

        // GET: DatPhong/Create
        public ActionResult Create(int? phongId)
        {
            if (phongId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var phong = db.Phong.Find(phongId);
            if (phong == null)
            {
                return HttpNotFound();
            }
            var userId = User.Identity.GetUserId();

            // Giả sử NguoiDungID là kiểu int
            int userIdInt;
            if (!int.TryParse(userId, out userIdInt))
            {
                return HttpNotFound();
            }

            var currentUser = db.NguoiDung.FirstOrDefault(u => u.NguoiDungID == userIdInt);
            if (currentUser == null)
            {
                return HttpNotFound();
            }
            var datPhong = new DatPhong
            {
                PhongID = phong.PhongID,
                NguoiDungID = currentUser.NguoiDungID,
                NgayDatPhong = DateTime.Now
            };
            ViewBag.TenNguoiDung = currentUser.TenNguoiDung;
            ViewBag.TenPhong = phong.LoaiP;
            return View(datPhong);
        }

        // POST: DatPhong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PhongID,NgayNhanPhong,NgayTraPhong,NguoiDungID")] DatPhong datPhong)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        datPhong.NgayDatPhong = DateTime.Now;
                        db.DatPhong.Add(datPhong);

                        // Cập nhật TinhTrang của Phòng
                        var phongToUpdate = db.Phong.Find(datPhong.PhongID);
                        if (phongToUpdate != null)
                        {
                            phongToUpdate.TinhTrang = false; // false đại diện cho "Đã đặt"
                        }

                        db.SaveChanges();
                        transaction.Commit();
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Có lỗi xảy ra trong quá trình đặt phòng.");
                    }
                }
            }

            var currentUser = db.NguoiDung.Find(datPhong.NguoiDungID);
            var phong = db.Phong.Find(datPhong.PhongID);
            ViewBag.TenNguoiDung = currentUser?.TenNguoiDung;
            ViewBag.TenPhong = phong?.LoaiP;
            return View(datPhong);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

