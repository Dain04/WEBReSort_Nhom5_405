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
    //[CheckUserLogin]
    public class HoaDonController : Controller
    {
        private QLKSEntities db = new QLKSEntities();

        // GET: HoaDon
        public ActionResult Index()
        {
            var hoaDon = db.HoaDon
                .Include(h => h.NguoiDung)
                .Include(h => h.Phong)
                .Include(h => h.Phong.LoaiPhong) // Thêm Include LoaiPhong
                .AsNoTracking()
                .ToList();

            foreach (var hd in hoaDon)
            {
                if (hd.PhongID.HasValue)
                {
                    var phong = db.Phong
                        .Include(p => p.LoaiPhong)
                        .FirstOrDefault(p => p.PhongID == hd.PhongID);

                    if (phong != null && hd.NgayTraPhong.HasValue && hd.NgayNhanPhong.HasValue)
                    {
                        int soNgayO = (hd.NgayTraPhong.Value - hd.NgayNhanPhong.Value).Days;
                        hd.TongTien = phong.Gia * soNgayO;
                    }
                }
            }

            return View(hoaDon);
        }

        // GET: HoaDon/Details/5

        public ActionResult IndexKH()
        {
            var hoaDon = db.HoaDon
                .Include(h => h.NguoiDung)
                .Include(h => h.Phong)
                .Include(h => h.Phong.LoaiPhong)
                .AsNoTracking()
                .ToList();

            foreach (var hd in hoaDon)
            {
                if (hd.PhongID.HasValue)
                {
                    var phong = db.Phong
                        .Include(p => p.LoaiPhong)
                        .FirstOrDefault(p => p.PhongID == hd.PhongID);

                    if (phong != null && hd.NgayTraPhong.HasValue && hd.NgayNhanPhong.HasValue)
                    {
                        int soNgayO = (hd.NgayTraPhong.Value - hd.NgayNhanPhong.Value).Days;
                        hd.TongTien = phong.Gia * soNgayO;
                    }

                    // Set default payment status if null
                    if (string.IsNullOrEmpty(hd.TrangThaiThanhToan))
                    {
                        hd.TrangThaiThanhToan = "Chưa thanh toán";
                    }
                }
            }
            return View(hoaDon);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Load HoaDon cùng với các related entities
            HoaDon hoaDon = db.HoaDon
                .Include(h => h.NguoiDung)
                .Include(h => h.Phong)
                .Include(h => h.Phong.LoaiPhong)
                .FirstOrDefault(h => h.HoaDonID == id);

            if (hoaDon == null)
            {
                return HttpNotFound();
            }

            // Load các thông tin liên quan
            ViewBag.TenKhachHang = hoaDon.NguoiDung?.TenNguoiDung;

            // Không cần phải load lại phòng vì đã Include ở trên
            if (hoaDon.Phong != null)
            {
                ViewBag.GiaPhong = hoaDon.Phong.Gia;

                // Đảm bảo LoaiPhong không null trước khi truy cập
                if (hoaDon.Phong.LoaiPhong != null)
                {
                    ViewBag.LoaiPhong = $"{hoaDon.Phong.LoaiPhong.TenLoai} - {hoaDon.Phong.PhongID}";
                }
                else
                {
                    ViewBag.LoaiPhong = "N/A";
                }
            }

            // Tính số ngày ở
            if (hoaDon.NgayTraPhong.HasValue && hoaDon.NgayNhanPhong.HasValue)
            {
                int soNgayO = (hoaDon.NgayTraPhong.Value - hoaDon.NgayNhanPhong.Value).Days;
                ViewBag.SoNgayO = soNgayO;

                // Tính tổng tiền
                if (hoaDon.Phong?.Gia != null)
                {
                    decimal tongTien = (decimal)(hoaDon.Phong.Gia * soNgayO);
                    ViewBag.TongTien = tongTien;
                    hoaDon.TongTien = tongTien; // Cập nhật tổng tiền vào model
                }
            }

            return View(hoaDon);
        }
        public ActionResult DetailsKH(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Load HoaDon cùng với các related entities
            HoaDon hoaDon = db.HoaDon
                .Include(h => h.NguoiDung)
                .Include(h => h.Phong)
                .Include(h => h.Phong.LoaiPhong)
                .FirstOrDefault(h => h.HoaDonID == id);

            if (hoaDon == null)
            {
                return HttpNotFound();
            }

            // Load các thông tin liên quan
            ViewBag.TenKhachHang = hoaDon.NguoiDung?.TenNguoiDung;

            // Không cần phải load lại phòng vì đã Include ở trên
            if (hoaDon.Phong != null)
            {
                ViewBag.GiaPhong = hoaDon.Phong.Gia;

                // Đảm bảo LoaiPhong không null trước khi truy cập
                if (hoaDon.Phong.LoaiPhong != null)
                {
                    ViewBag.LoaiPhong = $"{hoaDon.Phong.LoaiPhong.TenLoai} - {hoaDon.Phong.PhongID}";
                }
                else
                {
                    ViewBag.LoaiPhong = "N/A";
                }
            }

            // Tính số ngày ở
            if (hoaDon.NgayTraPhong.HasValue && hoaDon.NgayNhanPhong.HasValue)
            {
                int soNgayO = (hoaDon.NgayTraPhong.Value - hoaDon.NgayNhanPhong.Value).Days;
                ViewBag.SoNgayO = soNgayO;

                // Tính tổng tiền
                if (hoaDon.Phong?.Gia != null)
                {
                    decimal tongTien = (decimal)(hoaDon.Phong.Gia * soNgayO);
                    ViewBag.TongTien = tongTien;
                }
            }

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
            ViewBag.LoaiPhong = phong?.LoaiPhong;
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
            ViewBag.LoaiPhong = phong?.LoaiPhong;
            ViewBag.GiaPhong = phong?.Gia;

            // Tính số ngày ở
            int soNgayO = (hoaDon.NgayTraPhong.Value - hoaDon.NgayNhanPhong.Value).Days;

            // Tính tổng tiền
            decimal tongTien = (decimal)(ViewBag.GiaPhong * soNgayO);

            ViewBag.SoNgayO = soNgayO;
            ViewBag.TongTien = tongTien;
            return View(hoaDon);
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

            // Cập nhật trạng thái thanh toán của hóa đơn
            hoaDon.TrangThaiThanhToan = "Đã thanh toán";

            // Cập nhật trạng thái phòng
            if (hoaDon.PhongID.HasValue)
            {
                var phong = db.Phong.Find(hoaDon.PhongID.Value);
                if (phong != null)
                {
                    // Tìm trạng thái "Đã thanh toán" trong bảng TinhTrangPhong
                    var tinhTrangDaThanhToan = db.TinhTrangPhong.FirstOrDefault(t => t.IDTinhTrang == "7");
                    if (tinhTrangDaThanhToan != null)
                    {
                        phong.IDTinhTrang = tinhTrangDaThanhToan.IDTinhTrang;
                        db.Entry(phong).State = EntityState.Modified;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy trạng thái phòng 'Đã thanh toán'.";
                        return RedirectToAction("IndexKH");
                    }
                }
            }

            // Lưu thay đổi
            db.Entry(hoaDon).State = EntityState.Modified;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Thanh toán thành công!";
            return RedirectToAction("IndexKH");
        }

    }
}

