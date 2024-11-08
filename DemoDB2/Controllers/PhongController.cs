using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DemoDB2.Models;
using PagedList;
using System.Data.Entity;

namespace DemoDB2.Controllers
{
    [Route("api/phong")]
    public class PhongController : Controller
    {
        QLKSEntities database = new QLKSEntities();

        // API: Lấy danh sách tất cả các phòng
        [HttpGet]
        public JsonResult GetPhongList(int? TinhTrangID)
        {
            var phongs = database.Phong
                .Include(p => p.LoaiPhong)
                .Include(p => p.TinhTrangPhong);

            if (TinhTrangID.HasValue)
            {
                phongs = phongs.Where(p => p.IDTinhTrang == TinhTrangID.Value);
            }

            var phongList = phongs.Select(p => new
            {
                p.PhongID,
                p.Gia,
                LoaiPhong = p.LoaiPhong.TenLoai,
                TinhTrang = p.TinhTrangPhong.TenTinhTrang,
                p.ImagePhong
            }).ToList();

            return Json(phongList, JsonRequestBehavior.AllowGet);
        }


        // API: Lấy thông tin chi tiết của một phòng theo ID
        [HttpGet]
        public JsonResult GetPhongDetails(int id)
        {
            var phong = database.Phong
                .Include(p => p.LoaiPhong)
                .Include(p => p.TinhTrangPhong)
                .Where(p => p.PhongID == id)
                .Select(p => new
                {
                    p.PhongID,
                    p.Gia,
                    LoaiPhong = p.LoaiPhong.TenLoai,
                    TinhTrang = p.TinhTrangPhong.TenTinhTrang,
                    p.ImagePhong
                })
                .FirstOrDefault();

            if (phong == null)
            {
                return Json(new { error = "Phong not found" }, JsonRequestBehavior.AllowGet);
            }

            return Json(phong, JsonRequestBehavior.AllowGet);
        }


        // API: Thêm mới một phòng
        [HttpPost]
        public JsonResult CreatePhongApi(Phong pro)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image upload (if any)
                    if (pro.UploadImage != null && pro.UploadImage.ContentLength > 0)
                    {
                        string filename = Path.GetFileNameWithoutExtension(pro.UploadImage.FileName);
                        string extension = Path.GetExtension(pro.UploadImage.FileName);
                        filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);
                        pro.ImagePhong = "~/Content/images/" + filename;
                        pro.UploadImage.SaveAs(path);
                    }

                    database.Phong.Add(pro);
                    database.SaveChanges();
                    return Json(new { success = true, message = "Room created successfully" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            return Json(new { success = false, message = "Invalid model state" });
        }
    

        // API: Cập nhật thông tin một phòng
        [HttpPost]
        public JsonResult EditPhongApi(Phong phong)
        {
            if (ModelState.IsValid)
            {
                var existingPhong = database.Phong.Find(phong.PhongID);
                if (existingPhong == null)
                {
                    return Json(new { success = false, message = "Phong not found" });
                }

                // Update properties
                existingPhong.Gia = phong.Gia;
                existingPhong.IDLoai = phong.IDLoai;
                existingPhong.IDTinhTrang = phong.IDTinhTrang;

                // Handle image upload
                if (phong.UploadImage != null && phong.UploadImage.ContentLength > 0)
                {
                    string filename = Path.GetFileNameWithoutExtension(phong.UploadImage.FileName);
                    string extension = Path.GetExtension(phong.UploadImage.FileName);
                    filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);
                    phong.ImagePhong = "~/Content/images/" + filename;
                    phong.UploadImage.SaveAs(path);
                }
                else
                {
                    phong.ImagePhong = existingPhong.ImagePhong;
                }

                database.Entry(existingPhong).State = EntityState.Modified;
                database.SaveChanges();
                return Json(new { success = true, message = "Room updated successfully" });
            }
            return Json(new { success = false, message = "Invalid model state" });
        }


        // API: Xóa một phòng theo ID
        [HttpPost]
        public JsonResult DeletePhongApi(int id)
        {
            try
            {
                var phong = database.Phong.Find(id);
                if (phong == null)
                {
                    return Json(new { success = false, message = "Room not found" });
                }

                database.Phong.Remove(phong);
                database.SaveChanges();
                return Json(new { success = true, message = "Room deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult SelectLoai()
        {
            LoaiPhong se_cate = new LoaiPhong();
            se_cate.ListLoai = database.LoaiPhong.ToList<LoaiPhong>();
            return PartialView("SelectLoai", se_cate);
        }
        public ActionResult SelectTinhTrangPhong()
        {
            TinhTrangPhong se_cate = new TinhTrangPhong();
            se_cate.ListTinhTrang = database.TinhTrangPhong.ToList<TinhTrangPhong>();
            return PartialView("SelectTinhTrangPhong", se_cate);
        }
        public ActionResult CreatePhong()
        {
            ViewBag.LoaiPhongList = new SelectList(database.LoaiPhong, "IDLoai", "TenLoai");
            ViewBag.TinhTrangPhongList = new SelectList(database.TinhTrangPhong, "IDTinhTrang", "TenTinhTrang");
            return View();
        }

        [HttpPost]
        public ActionResult CreatePhong(Phong pro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (pro.UploadImage != null && pro.UploadImage.ContentLength > 0)
                    {
                        string filename = Path.GetFileNameWithoutExtension(pro.UploadImage.FileName);
                        string extension = Path.GetExtension(pro.UploadImage.FileName);
                        filename = filename + extension;
                        string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);
                        pro.ImagePhong = "~/Content/images/" + filename;
                        pro.UploadImage.SaveAs(path);
                    }



                    database.Phong.Add(pro);
                    database.SaveChanges();
                    return RedirectToAction("ViewPhong");
                }
                ViewBag.LoaiPhongList = new SelectList(database.LoaiPhong, "IDLoai", "TenLoai");
                ViewBag.TinhTrangPhongList = new SelectList(database.TinhTrangPhong, "IDTinhTrang", "TenTinhTrang");
                return View(pro);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                ViewBag.LoaiPhongList = new SelectList(database.LoaiPhong, "IDLoai", "TenLoai");
                ViewBag.TinhTrangPhongList = new SelectList(database.TinhTrangPhong, "IDTinhTrang", "TenTinhTrang");
                return View(pro);
            }
        }
        [HttpPost]
        public ActionResult ConfirmRoom(int id)
        {
            try
            {
                // Tìm phòng theo ID
                var phong = database.Phong.Find(id);
                if (phong == null)
                {
                    return HttpNotFound();
                }

                // Đảm bảo trạng thái hiện tại là Chờ Xác Nhận (ID = 2)
                if (phong.IDTinhTrang != 2)
                {
                    TempData["Error"] = "Chỉ có thể xác nhận phòng đang ở trạng thái Chờ Xác Nhận";
                    return RedirectToAction("ViewPhong");
                }

                // Cập nhật trạng thái phòng thành "Đã xác nhận" (ID = 3)
                phong.IDTinhTrang = 3;
                database.Entry(phong).State = EntityState.Modified;

                // Cập nhật trạng thái trong bảng DatPhong
                var datPhongList = database.DatPhong.Where(dp => dp.PhongID == id).ToList();
                foreach (var datPhong in datPhongList)
                {
                    datPhong.IDTinhTrang = 3; // Cập nhật trạng thái của DatPhong thành "Đã xác nhận"
                    database.Entry(datPhong).State = EntityState.Modified;
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                database.SaveChanges();

                // Thông báo thành công
                TempData["Success"] = "Đã xác nhận phòng thành công";

                // Refresh lại trang với các tham số hiện tại
                if (Request.QueryString["page"] != null)
                {
                    return RedirectToAction("ViewPhong", new
                    {
                        page = Request.QueryString["page"],
                        TinhTrangID = Request.QueryString["TinhTrangID"]
                    });
                }
                return RedirectToAction("ViewPhong");
            }
            catch (Exception ex)
            {
                // Log lỗi và thông báo
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("ViewPhong");
            }
        }
        public ActionResult ViewPhong(int? page, int? TinhTrangID)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var phongs = database.Phong
                .Include(p => p.LoaiPhong)
                .Include(p => p.TinhTrangPhong);

            if (TinhTrangID.HasValue)
            {
                phongs = phongs.Where(p => p.IDTinhTrang == TinhTrangID.Value);
            }

            var pagedPhongs = phongs
                .OrderBy(p => p.PhongID)
                .ToPagedList(pageNumber, pageSize);

            ViewBag.TinhTrangList = new SelectList(database.TinhTrangPhong, "IDTinhTrang", "TenTinhTrang");

            return View(pagedPhongs);
        }
        public ActionResult ViewPhongTieuChuan(int? page)
        {
            return ViewPhongKH(page, 2, null); // Giả sử ID 2 là cho Phòng Tiêu Chuẩn
        }

        public ActionResult ViewPhongVIP(int? page)
        {
            return ViewPhongKH(page, 1, null); // Giả sử ID 1 là cho Phòng VIP
        }
        public ActionResult ViewPhongKH(int? page, int? LoaiPhongID, int? TinhTrangID)
        {
            int pageSize = 6;
            int pageNumber = (page ?? 1);

            var phongs = database.Phong
                .Include(p => p.LoaiPhong)
                .Include(p => p.TinhTrangPhong);

            // Lọc theo loại phòng nếu có
            if (LoaiPhongID.HasValue)
            {
                phongs = phongs.Where(p => p.IDLoai == LoaiPhongID.Value);
            }

            // Lọc theo tình trạng nếu có
            if (TinhTrangID.HasValue)
            {
                phongs = phongs.Where(p => p.IDTinhTrang == TinhTrangID.Value);
            }

            // Thêm điều kiện để không hiển thị các phòng có tình trạng "Chờ xác nhận" (ID = 2)
            phongs = phongs.Where(p => p.IDTinhTrang != 2);

            var pagedPhongs = phongs.OrderBy(p => p.PhongID).ToPagedList(pageNumber, pageSize);

            // Thêm danh sách tình trạng vào ViewBag
            ViewBag.DanhSachTinhTrang = database.TinhTrangPhong.ToList();
            ViewBag.TinhTrangID = TinhTrangID; // Lưu lại tình trạng đã chọn

            // Các ViewBag khác vẫn giữ nguyên
            ViewBag.LoaiPhongID = LoaiPhongID;
            ViewBag.LoaiPhongList = new SelectList(database.LoaiPhong, "IDLoai", "TenLoai");

            return View(pagedPhongs);
        }
        public ActionResult EditPhong(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phong phong = database.Phong.Find(id);
            if (phong == null)
            {
                return HttpNotFound();
            }


            ViewBag.LoaiPhongList = new SelectList(database.LoaiPhong, "IDLoai", "TenLoai", phong.IDLoai);
            ViewBag.TinhTrangPhongList = new SelectList(database.TinhTrangPhong, "IDTinhTrang", "TenTinhTrang", phong.IDTinhTrang);

            return View(phong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPhong(Phong phong)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingPhong = database.Phong.Find(phong.PhongID);
                    if (existingPhong == null)
                    {
                        return HttpNotFound();
                    }

                    if (phong.UploadImage != null && phong.UploadImage.ContentLength > 0)
                    {
                        string filename = Path.GetFileNameWithoutExtension(phong.UploadImage.FileName);
                        string extension = Path.GetExtension(phong.UploadImage.FileName);
                        filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);
                        phong.ImagePhong = "~/Content/images/" + filename;
                        phong.UploadImage.SaveAs(path);
                    }
                    else
                    {
                        // Giữ nguyên ảnh cũ nếu không có ảnh mới được tải lên
                        phong.ImagePhong = existingPhong.ImagePhong;
                    }

                    // Cập nhật các trường khác
                    existingPhong.IDLoai = phong.IDLoai;
                    existingPhong.IDTinhTrang = phong.IDTinhTrang;
                    existingPhong.Gia = phong.Gia;
                    existingPhong.ImagePhong = phong.ImagePhong;

                    database.Entry(existingPhong).State = System.Data.Entity.EntityState.Modified;
                    database.SaveChanges();
                    return RedirectToAction("ViewPhong");
                }

                ViewBag.LoaiPhongList = new SelectList(database.LoaiPhong, "IDLoai", "TenLoai", phong.IDLoai);
                ViewBag.TinhTrangPhongList = new SelectList(database.TinhTrangPhong, "IDTinhTrang", "TenTinhTrang", phong.IDTinhTrang);
                return View(phong);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                ViewBag.LoaiPhongList = new SelectList(database.LoaiPhong, "TenLoai", "TenLoai", phong.LoaiPhong);
                return View(phong);
            }
        }
        public ActionResult DatPhong(int id)
        {
            Phong phong = database.Phong.Find(id);
            if (phong == null)
            {
                return HttpNotFound();
            }

            DatPhong datPhong = new DatPhong
            {
                PhongID = phong.PhongID,
                NgayDatPhong = DateTime.Now
            };

            ViewBag.NguoiDungID = new SelectList(database.NguoiDung, "NguoiDungID", "TenNguoiDung");
            return View(datPhong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DatPhong(DatPhong datPhong)
        {
            if (ModelState.IsValid)
            {
                datPhong.NgayDatPhong = DateTime.Now;
                database.DatPhong.Add(datPhong);

                var phong = database.Phong.Find(datPhong.PhongID);
                if (phong != null)
                {
                    // Tạo hóa đơn mới
                    var hoaDon = new HoaDon
                    {
                        KhachHangID = datPhong.NguoiDungID,
                        PhongID = datPhong.PhongID,
                        NgayNhanPhong = datPhong.NgayNhanPhong,
                        NgayTraPhong = datPhong.NgayTraPhong,
                        NguoiDungID = datPhong.NguoiDungID,  // Đảm bảo đây là ID hợp lệ
                    };

                    database.HoaDon.Add(hoaDon);
                    // Cập nhật trạng thái phòng thành "Chờ xác nhận" (ID = 2)
                    phong.IDTinhTrang = 2; // Giả sử ID 2 là trạng thái "Chờ xác nhận"
                    database.Entry(phong).State = EntityState.Modified;
                    datPhong.ImagePhong = phong.ImagePhong; 
                    datPhong.IDTinhTrang = phong.IDTinhTrang;
                }

                database.SaveChanges();

                TempData["SuccessMessage"] = "Đặt phòng thành công. Hóa đơn đã được tạo.";
                // Chuyển hướng về trang ViewPhongVIP hoặc ViewPhongTieuChuan
                if (phong.IDLoai == 1) // Giả sử ID 1 là cho phòng VIP
                {
                    return RedirectToAction("ViewPhongVIP");
                }
                else if (phong.IDLoai == 2) // Giả sử ID 2 là cho phòng tiêu chuẩn
                {
                    return RedirectToAction("ViewPhongTieuChuan");
                }
            }

            return View(datPhong);
        }
        public ActionResult ChiTietDatPhong(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy danh sách đặt phòng của người dùng
            var datPhongList = database.DatPhong
                .Include(dp => dp.Phong)
                .Include(dp => dp.TinhTrangPhong)
                .Where(dp => dp.NguoiDungID == id)
                .OrderByDescending(dp => dp.NgayDatPhong)
                .ToList();

            if (datPhongList == null || !datPhongList.Any())
            {
                TempData["Message"] = "Không có phiếu đặt phòng nào.";
                return RedirectToAction("Index", "Home");
            }

            return View(datPhongList);
        }
        public ActionResult DeletePhong(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phong phong = database.Phong.Find(id);
            if (phong == null)
            {
                return HttpNotFound();
            }
            return View(phong);
        }

        [HttpPost, ActionName("DeletePhong")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePhongConfirmed(int id)
        {
            try
            {
                Phong phong = database.Phong.Find(id);
                if (phong == null)
                {
                    return HttpNotFound();
                }

                // Check if the room is currently booked
                var bookings = database.DatPhong.Where(dp => dp.PhongID == id && dp.NgayTraPhong >= DateTime.Now).ToList();
                if (bookings.Any())
                {
                    ModelState.AddModelError("", "Không thể xóa phòng này vì đang có đặt phòng.");
                    return View(phong);
                }

                // Delete associated bookings and invoices
                var relatedBookings = database.DatPhong.Where(dp => dp.PhongID == id);
                foreach (var booking in relatedBookings)
                {
                    var relatedInvoices = database.HoaDon.Where(hd => hd.PhongID == id && hd.KhachHangID == booking.NguoiDungID);
                    database.HoaDon.RemoveRange(relatedInvoices);
                }
                database.DatPhong.RemoveRange(relatedBookings);

                // Delete the room
                database.Phong.Remove(phong);
                database.SaveChanges();

                return RedirectToAction("ViewPhong");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi xóa phòng: " + ex.Message);
                return View(database.Phong.Find(id));
            }
        }
    }
}
