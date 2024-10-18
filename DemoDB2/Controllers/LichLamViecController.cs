using DemoDB2.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DemoDB2.Controllers
{
    [AuthenticationFilter]
    public class LichLamViecController : Controller
    {
        private QLKSEntities database = new QLKSEntities();

        public ActionResult DangKyLichLamViec()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKyLichLamViec(LichLamViec lichLamViec)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (lichLamViec.SoNgayLamViec < 25 || lichLamViec.SoNgayLamViec > 30)
                    {
                        ModelState.AddModelError("SoNgayLamViec", "Số ngày làm việc phải từ 25 đến 30 ngày.");
                        return View(lichLamViec);
                    }
                    // Lấy NhanVienID của người đăng nhập hiện tại
                    int nhanVienID = (int)Session["NhanVienID"];
                    lichLamViec.NhanVienID = nhanVienID;

                    // Tính toán ThoiGianBatDau và ThoiGianKetThuc dựa trên CaLamViec
                    switch (lichLamViec.CaLamViec)
                    {
                        case 1:
                            lichLamViec.ThoiGianBatDau = new TimeSpan(6, 0, 0);
                            lichLamViec.ThoiGianKetThuc = new TimeSpan(12, 0, 0);
                            break;
                        case 2:
                            lichLamViec.ThoiGianBatDau = new TimeSpan(12, 0, 0);
                            lichLamViec.ThoiGianKetThuc = new TimeSpan(18, 0, 0);
                            break;
                        case 3:
                            lichLamViec.ThoiGianBatDau = new TimeSpan(18, 0, 0);
                            lichLamViec.ThoiGianKetThuc = new TimeSpan(0, 0, 0);
                            break;
                        case 4:
                            lichLamViec.ThoiGianBatDau = new TimeSpan(0, 0, 0);
                            lichLamViec.ThoiGianKetThuc = new TimeSpan(6, 0, 0);
                            break;
                    }

                    // Kiểm tra xem đã có lịch làm việc cho tháng và năm này chưa
                    var existingSchedule = database.LichLamViec.FirstOrDefault(l =>
                        l.NhanVienID == nhanVienID &&
                        l.Thang == lichLamViec.Thang &&
                        l.Nam == lichLamViec.Nam);

                    if (existingSchedule != null)
                    {
                        // Cập nhật lịch làm việc hiện có
                        existingSchedule.SoNgayLamViec = lichLamViec.SoNgayLamViec;
                        existingSchedule.CaLamViec = lichLamViec.CaLamViec;
                        existingSchedule.ThoiGianBatDau = lichLamViec.ThoiGianBatDau;
                        existingSchedule.ThoiGianKetThuc = lichLamViec.ThoiGianKetThuc;
                        database.SaveChanges();
                    }
                    else
                    {
                        // Thêm lịch làm việc mới
                        database.LichLamViec.Add(lichLamViec);
                        database.SaveChanges();
                    }

                    // Tính và cập nhật lương
                    decimal tongLuong = TinhLuong(lichLamViec);
                    var luong = database.Luong.FirstOrDefault(l => l.NhanVienID == nhanVienID && l.Thang == lichLamViec.Thang && l.Nam == lichLamViec.Nam);
                    if (luong == null)
                    {
                        luong = new Luong
                        {
                            NhanVienID = nhanVienID,
                            Thang = lichLamViec.Thang,
                            Nam = lichLamViec.Nam,
                            LuongMotGio = 35000,
                            TongLuong = tongLuong
                        };
                        database.Luong.Add(luong);
                    }
                    else
                    {
                        luong.TongLuong = tongLuong;
                    }
                    database.SaveChanges();

                    return RedirectToAction("XemLichLamViec");
                }

                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi đăng ký lịch làm việc: " + ex.Message);
                }
            }

            return View(lichLamViec);
        }

        private decimal TinhLuong(LichLamViec lichLamViec)
        {
            int soGioLamViecMoiNgay = 6; // Mỗi ca làm việc 6 tiếng
            decimal luongMotGio = 35000;
            decimal tongLuong = lichLamViec.SoNgayLamViec * soGioLamViecMoiNgay * luongMotGio;
            return tongLuong;
        }

        public ActionResult XemLichLamViec()
        {
            try
            {
                int nhanVienID = (int)Session["NhanVienID"];
                var lichLamViec = database.LichLamViec.Where(l => l.NhanVienID == nhanVienID).ToList();
                return View(lichLamViec);
            }
            catch (InvalidCastException)
            {
                return RedirectToAction("LoginNV", "LoginNhanVien");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                database.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class AuthenticationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["NhanVienID"] == null)
            {
                filterContext.Result = new RedirectResult("~/LoginNhanVien/LoginNV");
            }
        }
    }
   
}