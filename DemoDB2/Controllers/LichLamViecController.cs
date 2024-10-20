using DemoDB2.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace DemoDB2.Controllers
{
    [AuthenticationFilter]
    public class LichLamViecController : Controller
    {
        private QLKSEntities database = new QLKSEntities();

        public ActionResult DangKyLichLamViec()
        {
            return View(new LichLamViec());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKyLichLamViec(LichLamViec lichLamViec)
        {
            try
            {
                // Kiểm tra Session trước
                if (Session["NhanVienID"] == null)
                {
                    ModelState.AddModelError("", "Vui lòng đăng nhập lại!");
                    return View(lichLamViec);
                }

                // Gán NhanVienID từ Session
                lichLamViec.NhanVienID = (int)Session["NhanVienID"];

                // Tính số ca làm việc
                int soCa = 0;
                if (lichLamViec.CaSang) soCa++;
                if (lichLamViec.CaChieu) soCa++;
                if (lichLamViec.CaToi) soCa++;
                if (lichLamViec.CaDem) soCa++;

                // Validate số ca làm việc
                if (soCa == 0)
                {
                    ModelState.AddModelError("", "Vui lòng chọn ít nhất 1 ca làm việc!");
                    return View(lichLamViec);
                }

                if (soCa > 2)
                {
                    ModelState.AddModelError("", "Chỉ được đăng ký tối đa 2 ca một ngày!");
                    return View(lichLamViec);
                }

                // Validate ngày tháng
                if (lichLamViec.Ngay < 1 || lichLamViec.Ngay > 31)
                {
                    ModelState.AddModelError("Ngay", "Ngày không hợp lệ!");
                    return View(lichLamViec);
                }

                if (lichLamViec.Thang < 1 || lichLamViec.Thang > 12)
                {
                    ModelState.AddModelError("Thang", "Tháng không hợp lệ!");
                    return View(lichLamViec);
                }

                if (lichLamViec.Nam < 2024)
                {
                    ModelState.AddModelError("Nam", "Năm không hợp lệ!");
                    return View(lichLamViec);
                }

                // Kiểm tra lịch làm việc đã tồn tại
                bool existingSchedule = database.LichLamViec.Any(l =>
                    l.NhanVienID == lichLamViec.NhanVienID &&
                    l.Ngay == lichLamViec.Ngay &&
                    l.Thang == lichLamViec.Thang &&
                    l.Nam == lichLamViec.Nam);

                if (existingSchedule)
                {
                    ModelState.AddModelError("", "Bạn đã đăng ký lịch làm việc cho ngày này!");
                    return View(lichLamViec);
                }

                // Cập nhật số ca làm việc
                lichLamViec.SoCaLamViec = soCa;

                // Thêm vào database
                database.LichLamViec.Add(lichLamViec);
                database.SaveChanges();

                TempData["Success"] = "Đăng ký lịch làm việc thành công!";
                return RedirectToAction("XemLichLamViec");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
                return View(lichLamViec);
            }
        }

        public ActionResult XemLichLamViec()
        {
            try
            {
                if (Session["NhanVienID"] == null || Session["TenChucVu"] == null)
                {
                    return RedirectToAction("LoginNV", "LoginNhanVien");
                }

                int nhanVienID = (int)Session["NhanVienID"];
                string tenChucVu = Session["TenChucVu"].ToString();

            
                System.Diagnostics.Debug.WriteLine("Chức vụ hiện tại: " + tenChucVu);

                if (tenChucVu.Equals("Quản Lý", StringComparison.OrdinalIgnoreCase) ||
                    tenChucVu.Equals("Giám Đốc", StringComparison.OrdinalIgnoreCase))
                {
                    var allLichLamViec = database.LichLamViec
                        .Include(l => l.NhanVien)
                        .OrderByDescending(l => l.Nam)
                        .ThenByDescending(l => l.Thang)
                        .ThenByDescending(l => l.Ngay)
                        .ToList();
                    ViewBag.IsManager = true;
                    return View(allLichLamViec);
                }
                else
                {
                    var lichLamViec = database.LichLamViec
                        .Where(l => l.NhanVienID == nhanVienID)
                        .OrderByDescending(l => l.Nam)
                        .ThenByDescending(l => l.Thang)
                        .ThenByDescending(l => l.Ngay)
                        .ToList();
                    ViewBag.IsManager = false;
                    return View(lichLamViec);
                }
            }
            catch (Exception)
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
