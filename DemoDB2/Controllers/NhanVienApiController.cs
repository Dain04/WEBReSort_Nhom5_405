using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DemoDB2.Models;
using System.Data.Entity;
using System.Web;
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
