using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DemoDB2.Models;

namespace DemoDB2.Controllers
{
    public class HoaDonController : Controller
    {
        private QLKSEntities db = new QLKSEntities();

        // GET: HoaDon
        public ActionResult Index()
        {
            var hoaDon = db.HoaDon
                .Include(h => h.NguoiDung)
                .Include(h => h.Phong)
                .AsNoTracking() // Đảm bảo lấy dữ liệu mới nhất
                .ToList();

            foreach (var hd in hoaDon)
            {
                // Load các thông tin liên quan
                var phong = db.Phong.Find(hd.PhongID);

                // Tính số ngày ở
                int soNgayO = (hd.NgayTraPhong.Value - hd.NgayNhanPhong.Value).Days;

                // Tính tổng tiền
                decimal tongTien = (decimal)(phong.Gia * soNgayO);

                hd.TongTien = tongTien;
            }

            return View(hoaDon);
        }

        // GET: HoaDon/Details/5
        public ActionResult IndexKH()
        {
            var hoaDon = db.HoaDon
                .Include(h => h.NguoiDung)
                .Include(h => h.Phong)
                .AsNoTracking() // Đảm bảo lấy dữ liệu mới nhất
                .ToList();

            foreach (var hd in hoaDon)
            {
                // Load các thông tin liên quan
                var phong = db.Phong.Find(hd.PhongID);

                // Tính số ngày ở
                int soNgayO = (hd.NgayTraPhong.Value - hd.NgayNhanPhong.Value).Days;

                // Tính tổng tiền
                decimal tongTien = (decimal)(phong.Gia * soNgayO);

                hd.TongTien = tongTien;
            }

            return View(hoaDon);
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoaDon hoaDon = db.HoaDon.Find(id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }

            // Load các thông tin liên quan
            var khachHang = db.NguoiDung.Find(hoaDon.KhachHangID);
            var phong = db.Phong.Find(hoaDon.PhongID);

            ViewBag.TenKhachHang = khachHang?.TenNguoiDung;
            ViewBag.LoaiPhong = phong?.LoaiP;
            ViewBag.GiaPhong = phong?.Gia;

            // Tính số ngày ở
            int soNgayO = (hoaDon.NgayTraPhong.Value - hoaDon.NgayNhanPhong.Value).Days;

            // Tính tổng tiền
            decimal tongTien = (decimal)(ViewBag.GiaPhong * soNgayO);

            ViewBag.SoNgayO = soNgayO;
            ViewBag.TongTien = tongTien;

            return View(hoaDon);
        }
        public ActionResult DetailsKH(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoaDon hoaDon = db.HoaDon.Find(id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }

            // Load các thông tin liên quan
            var khachHang = db.NguoiDung.Find(hoaDon.KhachHangID);
            var phong = db.Phong.Find(hoaDon.PhongID);

            ViewBag.TenKhachHang = khachHang?.TenNguoiDung;
            ViewBag.LoaiPhong = phong?.LoaiP;
            ViewBag.GiaPhong = phong?.Gia;

            // Tính số ngày ở
            int soNgayO = (hoaDon.NgayTraPhong.Value - hoaDon.NgayNhanPhong.Value).Days;

            // Tính tổng tiền
            decimal tongTien = (decimal)(ViewBag.GiaPhong * soNgayO);

            ViewBag.SoNgayO = soNgayO;
            ViewBag.TongTien = tongTien;

            return View(hoaDon);
        }

        // GET: HoaDon/Create
        public ActionResult Create()
        {
            ViewBag.NguoiDungID = new SelectList(db.NguoiDung, "NguoiDungID", "TenNguoiDung");
            ViewBag.PhongID = new SelectList(db.Phong, "PhongID", "LoaiP");
            return View();
        }

        // POST: HoaDon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HoaDonID,KhachHangID,PhongID,NgayNhanPhong,NgayTraPhong,TongTien,IDDichVuSuDung,NguoiDungID,NhanVienID")] HoaDon hoaDon)
        {
            if (ModelState.IsValid)
            {
                db.HoaDon.Add(hoaDon);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.NguoiDungID = new SelectList(db.NguoiDung, "NguoiDungID", "TenNguoiDung", hoaDon.NguoiDungID);
            ViewBag.PhongID = new SelectList(db.Phong, "PhongID", "LoaiP", hoaDon.PhongID);
            return View(hoaDon);
        }

        // GET: HoaDon/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoaDon hoaDon = db.HoaDon.Find(id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }

            // Replace ViewBag with DropDownListFor
            ViewBag.NguoiDungID = new SelectList(db.NguoiDung, "NguoiDungID", "TenNguoiDung", hoaDon.NguoiDungID);
            ViewBag.PhongID = new SelectList(db.Phong, "PhongID", "LoaiP", hoaDon.PhongID);
            return View(hoaDon);
        }


        // POST: HoaDon/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HoaDonID,KhachHangID,PhongID,NgayNhanPhong,NgayTraPhong,TongTien,IDDichVuSuDung,NguoiDungID,NhanVienID")] HoaDon hoaDon)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hoaDon).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NguoiDungID = new SelectList(db.NguoiDung, "NguoiDungID", "TenNguoiDung", hoaDon.NguoiDungID);
            ViewBag.PhongID = new SelectList(db.Phong, "PhongID", "LoaiP", hoaDon.PhongID);
            return View(hoaDon);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoaDon hoaDon = db.HoaDon.Find(id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }

            // Load các thông tin liên quan
            var khachHang = db.NguoiDung.Find(hoaDon.KhachHangID);
            var phong = db.Phong.Find(hoaDon.PhongID);

            ViewBag.TenKhachHang = khachHang?.TenNguoiDung;
            ViewBag.LoaiPhong = phong?.LoaiP;
            ViewBag.GiaPhong = phong?.Gia;

            // Tính số ngày ở
            int soNgayO = (hoaDon.NgayTraPhong.Value - hoaDon.NgayNhanPhong.Value).Days;

            // Tính tổng tiền
            decimal tongTien = (decimal)(ViewBag.GiaPhong * soNgayO);

            ViewBag.SoNgayO = soNgayO;
            ViewBag.TongTien = tongTien;

            return View(hoaDon);
        }

        // POST: HoaDon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HoaDon hoaDon = db.HoaDon.Find(id);
            db.HoaDon.Remove(hoaDon);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmPayment(int id)
        {
            var hoaDon = db.HoaDon.Find(id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    hoaDon.TrangThaiThanhToan = "Đã thanh toán";

                    // Cập nhật TinhTrang của Phòng
                    var phong = db.Phong.Find(hoaDon.PhongID);
                    if (phong != null)
                    {
                        phong.TinhTrang = true; // true đại diện cho "Trống"
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    TempData["SuccessMessage"] = "Thanh toán thành công!";
                    return RedirectToAction("IndexKH");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình thanh toán.";
                    return RedirectToAction("IndexKH");
                }
            }
        }

        public ActionResult Payment(int id)
        {
            var hoaDon = db.HoaDon.Find(id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }

            if (hoaDon.TrangThaiThanhToan == "Đã thanh toán")
            {
                TempData["ErrorMessage"] = "Hóa đơn này đã được thanh toán.";
                return RedirectToAction("IndexKH");
            }

            // Generate a fake QR code (in reality, this would be generated based on payment information)
            string qrCodeData = $"PAYMENT:{hoaDon.HoaDonID}:{hoaDon.TongTien}";
            ViewBag.QRCodeData = qrCodeData;
            // Load các thông tin liên quan
            var khachHang = db.NguoiDung.Find(hoaDon.KhachHangID);
            var phong = db.Phong.Find(hoaDon.PhongID);

            ViewBag.TenKhachHang = khachHang?.TenNguoiDung;
            ViewBag.LoaiPhong = phong?.LoaiP;
            ViewBag.GiaPhong = phong?.Gia;

            // Tính số ngày ở
            int soNgayO = (hoaDon.NgayTraPhong.Value - hoaDon.NgayNhanPhong.Value).Days;

            // Tính tổng tiền
            decimal tongTien = (decimal)(ViewBag.GiaPhong * soNgayO);

            ViewBag.SoNgayO = soNgayO;
            ViewBag.TongTien = tongTien;
            return View(hoaDon);
        }
    }
}
