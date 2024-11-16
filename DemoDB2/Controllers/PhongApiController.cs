using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Data.Entity;
using DemoDB2.Models;
using System.Web;

namespace DemoDB2.Controllers
{
    [RoutePrefix("api/phong")]
    public class PhongApiController : ApiController
    {
        private QLKSEntities database = new QLKSEntities();

        // GET: api/phong/GetPhongList
        [Route("GetPhongList")]
        [HttpGet]
        public IHttpActionResult GetPhongList(int? TinhTrangID)
        {
            try
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
                    LoaiPhong = p.LoaiPhong.TenLoai.Trim(),
                    TinhTrang = p.TinhTrangPhong.TenTinhTrang.Trim(),
                    p.ImagePhong,
                    // Thêm các thông tin khác
                    IDLoai = p.IDLoai,
                    IDTinhTrang = p.IDTinhTrang,
                    GiaFormatted = string.Format("{0:N0} VNĐ", p.Gia) // Format giá tiền
                }).ToList();

                return Ok(phongList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/phong/GetPhongDetails/5
        [Route("GetPhongDetails/{id}")]
        [HttpGet]
        public IHttpActionResult GetPhongDetails(int id)
        {
            try
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
                        p.ImagePhong,
                        p.IDLoai,
                        p.IDTinhTrang
                    })
                    .FirstOrDefault();

                if (phong == null)
                {
                    return NotFound();
                }

                // Format dữ liệu sau khi đã query từ database
                var response = new
                {
                    Success = true,
                    Data = new
                    {
                        phong.PhongID,
                        phong.Gia,
                        GiaFormatted = string.Format("{0:N0} VNĐ", phong.Gia),
                        LoaiPhong = phong.LoaiPhong.Trim(),
                        TinhTrang = phong.TinhTrang.Trim(),
                        ImagePhong = VirtualPathUtility.ToAbsolute(phong.ImagePhong),
                        phong.IDLoai,
                        phong.IDTinhTrang,
                        CreatedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                    },
                    Message = "Room details retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Error retrieving room details: " + ex.Message));
            }
        }
        // POST: api/phong/CreatePhong
        [Route("CreatePhong")]
        [HttpPost]
        public IHttpActionResult CreatePhong([FromBody] Phong phong)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Handle image upload (if any)
                if (phong.UploadImage != null && phong.UploadImage.ContentLength > 0)
                {
                    string filename = Path.GetFileNameWithoutExtension(phong.UploadImage.FileName);
                    string extension = Path.GetExtension(phong.UploadImage.FileName);
                    filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/"), filename);
                    phong.ImagePhong = "~/Content/images/" + filename;
                    phong.UploadImage.SaveAs(path);
                }

                database.Phong.Add(phong);
                database.SaveChanges();

                return Ok(new { success = true, message = "Room created successfully" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/phong/EditPhong
        [Route("EditPhong")]
        [HttpPost]
        public IHttpActionResult EditPhong([FromBody] Phong phong)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingPhong = database.Phong.Find(phong.PhongID);
                if (existingPhong == null)
                {
                    return NotFound();
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
                    string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Content/images/"), filename);
                    existingPhong.ImagePhong = "~/Content/images/" + filename;
                    phong.UploadImage.SaveAs(path);
                }

                database.Entry(existingPhong).State = EntityState.Modified;
                database.SaveChanges();

                return Ok(new { success = true, message = "Room updated successfully" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/phong/DeletePhong/5
        [Route("DeletePhong/{id}")]
        [HttpPost]
        public IHttpActionResult DeletePhong(int id)
        {
            try
            {
                var phong = database.Phong.Find(id);
                if (phong == null)
                {
                    return NotFound();
                }

                database.Phong.Remove(phong);
                database.SaveChanges();

                return Ok(new { success = true, message = "Room deleted successfully" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/phong/GetLoaiPhong
        [Route("GetLoaiPhong")]
        [HttpGet]
        public IHttpActionResult GetLoaiPhong()
        {
            try
            {
                var loaiPhongList = database.LoaiPhong
                    .Select(l => new
                    {
                        l.IDLoai,
                        l.TenLoai
                    })
                    .ToList();

                return Ok(loaiPhongList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/phong/GetTinhTrangPhong
        [Route("GetTinhTrangPhong")]
        [HttpGet]
        public IHttpActionResult GetTinhTrangPhong()
        {
            try
            {
                var tinhTrangList = database.TinhTrangPhong
                    .Select(t => new
                    {
                        t.IDTinhTrang,
                        t.TenTinhTrang
                    })
                    .ToList();

                return Ok(tinhTrangList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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
}