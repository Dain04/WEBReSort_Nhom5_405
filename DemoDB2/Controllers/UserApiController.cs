using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DemoDB2.Models;
using System.Data.Entity;
using System.IO;
using System.Web;

namespace DemoDB2.Controllers
{
    [RoutePrefix("api/user")]
    public class UserApiController : ApiController
    {
        private QLKSEntities database = new QLKSEntities();
        //get : api/user/GetAllUser
        [Route("GetAllUsers")]
        [HttpGet]
        public IHttpActionResult GetAllUsers()
        {
            try
            {
                var allUsers = database.NguoiDung
                    .Select(p => new
                    {
                        p.TenNguoiDung,
                        p.NguoiDungID,
                        p.SoDienThoai,
                        p.ImageUser,
                        p.Email,
                        p.DiaChi
                    })
                    .ToList();
                return Ok(allUsers);
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
        // Update user information
        [Route("UpdateUser/{id}")]
        [HttpPut]
        public IHttpActionResult UpdateUser(int id)
        {
            try
            {
                var existingUser = database.NguoiDung.Find(id);
                if (existingUser == null)
                    return NotFound();

                var form = HttpContext.Current.Request.Form;
                if (form["TenNguoiDung"] != null)
                    existingUser.TenNguoiDung = form["TenNguoiDung"];

                if (form["SoDienThoai"] != null)
                    existingUser.SoDienThoai = form["SoDienThoai"];

                if (form["Email"] != null)
                    existingUser.Email = form["Email"];

                if (form["DiaChi"] != null)
                    existingUser.DiaChi = form["DiaChi"];

                // Update user image if provided
                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    var file = HttpContext.Current.Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(file.FileName).ToLower();

                        if (!allowedExtensions.Contains(extension))
                            return BadRequest("Chỉ chấp nhận file ảnh định dạng: " + string.Join(", ", allowedExtensions));

                        if (file.ContentLength > 5 * 1024 * 1024)
                            return BadRequest("Kích thước file không được vượt quá 5MB");

                        if (!string.IsNullOrEmpty(existingUser.ImageUser))
                        {
                            var oldPath = HttpContext.Current.Server.MapPath(existingUser.ImageUser);
                            if (File.Exists(oldPath))
                                File.Delete(oldPath);
                        }

                        string filename = Path.GetFileNameWithoutExtension(file.FileName) + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/images/"), filename);
                        file.SaveAs(path);
                        existingUser.ImageUser = "~/Content/images/" + filename;
                    }
                }

                database.Entry(existingUser).State = EntityState.Modified;
                database.SaveChanges();

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật thông tin người dùng thành công",
                    data = existingUser
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Lỗi khi cập nhật người dùng: " + ex.Message));
            }
        }
    }
}
