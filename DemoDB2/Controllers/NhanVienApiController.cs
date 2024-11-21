using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DemoDB2.Models;
using System.Data.Entity;
using System.Web;
using System.IO;

namespace DemoDB2.Controllers
{
    [RoutePrefix("api/nhanvien")]
    public class NhanVienApiController : ApiController
    {
        private QLKSEntities database = new QLKSEntities();

        //Get all nhanvien
        [Route("GetAllNhanvien")]
        [HttpGet]
        public IHttpActionResult GetAllNhanVien()
        {
            try
            {
                var allNhanvien = database.NhanVien
                     .Include(p => p.ChucVu)
                    .Select(p => new
                    {
                        p.Ten,
                        p.NhanVienID,
                       ChucVu = p.ChucVu.TenChucVu.Trim(),
                        p.SoDienThoai,
                        p.Email,
                        p.DiaChi,

                    })
                    .ToList();
                return Ok(allNhanvien);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        // Get a single user by ID
        [Route("GetUser/{id}")]
        [HttpGet]
        public IHttpActionResult GetUser(int id)
        {
            try
            {
                var user = database.NguoiDung
                    .Where(u => u.NguoiDungID == id)
                    .Select(p => new
                    {
                        p.TenNguoiDung,
                        p.NguoiDungID,
                        p.SoDienThoai,
                        p.ImageUser,
                        p.Email,
                        p.DiaChi
                    })
                    .FirstOrDefault();

                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [Route("EditNhanVien/{id}")]
        [HttpPut]
        public IHttpActionResult EditNhanVien(int id)
        {
            try
            {
                // Tìm nhân viên theo ID
                var existingNhanVien = database.NhanVien.Find(id);
                if (existingNhanVien == null)
                    return NotFound();

                // Cập nhật thông tin từ form data
                if (HttpContext.Current.Request.Form["Ten"] != null)
                    existingNhanVien.Ten = HttpContext.Current.Request.Form["Ten"];

                if (HttpContext.Current.Request.Form["DiaChi"] != null)
                    existingNhanVien.DiaChi = HttpContext.Current.Request.Form["DiaChi"];

                if (HttpContext.Current.Request.Form["SoDienThoai"] != null)
                    existingNhanVien.SoDienThoai = HttpContext.Current.Request.Form["SoDienThoai"];

                if (HttpContext.Current.Request.Form["Email"] != null)
                    existingNhanVien.Email = HttpContext.Current.Request.Form["Email"];

                if (HttpContext.Current.Request.Form["ChucVuID"] != null)
                    existingNhanVien.ChucVuID = Convert.ToInt32(HttpContext.Current.Request.Form["ChucVuID"]);

                // Xử lý upload ảnh mới nếu có
                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    var file = HttpContext.Current.Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        // Kiểm tra định dạng file
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(file.FileName).ToLower();
                        if (!allowedExtensions.Contains(extension))
                            return BadRequest("Chỉ chấp nhận file ảnh định dạng: " + string.Join(", ", allowedExtensions));

                        // Kiểm tra kích thước file
                        if (file.ContentLength > 5 * 1024 * 1024)
                            return BadRequest("Kích thước file không được vượt quá 5MB");

                        // Xóa ảnh cũ nếu tồn tại
                        if (!string.IsNullOrEmpty(existingNhanVien.ImageNhanVien))
                        {
                            var oldImagePath = HttpContext.Current.Server.MapPath(existingNhanVien.ImageNhanVien);
                            if (File.Exists(oldImagePath))
                                File.Delete(oldImagePath);
                        }

                        string filename = Path.GetFileNameWithoutExtension(file.FileName);
                        filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/images/"), filename);
                        file.SaveAs(path);
                        existingNhanVien.ImageNhanVien = "~/Content/images/" + filename;
                    }
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                database.Entry(existingNhanVien).State = EntityState.Modified;
                database.SaveChanges();

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật nhân viên thành công",
                    data = new
                    {
                        existingNhanVien.NhanVienID,
                        existingNhanVien.Ten,
                        existingNhanVien.DiaChi,
                        existingNhanVien.SoDienThoai,
                        existingNhanVien.Email,
                        existingNhanVien.ChucVuID,
                        existingNhanVien.ImageNhanVien
                    }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Lỗi khi cập nhật nhân viên: " + ex.Message));
            }
        }
        // Delete a user by ID
        [Route("DeleteUser/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteUser(int id)
        {
            try
            {
                var user = database.NguoiDung.Find(id);
                if (user == null)
                    return NotFound();

                // Delete user image if exists
                if (!string.IsNullOrEmpty(user.ImageUser))
                {
                    var path = HttpContext.Current.Server.MapPath(user.ImageUser);
                    if (File.Exists(path))
                        File.Delete(path);
                }

                database.NguoiDung.Remove(user);
                database.SaveChanges();

                return Ok(new
                {
                    success = true,
                    message = "Xóa người dùng thành công"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Lỗi khi xóa người dùng: " + ex.Message));
            }
        }
        //GetQLý
        [Route("GetAllTheoChucVu")]
        [HttpGet]
        public IHttpActionResult GetAllTheoChucVu()
        {
            try
            {
                int chucVuId = 1; // ID chức vụ cần lọc
                var allTheoChucVu = database.NhanVien
                    .Include(p => p.ChucVu) // Bao gồm thông tin chức vụ
                    .Where(p => p.ChucVuID == chucVuId) // Lọc theo ID chức vụ
                    .Select(p => new
                    {
                        p.Ten,
                        ChucVu = p.ChucVu != null ? p.ChucVu.TenChucVu.Trim() : "Chưa có chức vụ", // Kiểm tra null
                p.SoDienThoai,
                        p.DiaChi,
                        p.Email
                    })
                    .ToList();
                return Ok(allTheoChucVu); // Trả về danh sách nhân viên theo chức vụ
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); // Xử lý lỗi
            }
        }
    }
}
