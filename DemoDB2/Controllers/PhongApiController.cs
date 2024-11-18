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
        public IHttpActionResult GetPhongList(int? TinhTrangID, int page = 1, int pageSize = 10)
        {
            var phongs = database.Phong
                .Include(p => p.LoaiPhong)
                .Include(p => p.TinhTrangPhong);

            // Lọc theo tình trạng
            if (TinhTrangID.HasValue)
            {
                phongs = phongs.Where(p => p.IDTinhTrang == TinhTrangID.Value);
            }

            var phongList = phongs
                .OrderBy(p => p.PhongID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.PhongID,
                    p.Gia,
                    p.IDLoai, // Thêm IDLoai vào response
                    LoaiPhong = p.LoaiPhong.TenLoai.Trim(),
                    TinhTrang = p.TinhTrangPhong.TenTinhTrang.Trim(),
                    p.ImagePhong,
                }).ToList();

            return Ok(phongList);
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
        [Route("CreatePhong")]
        [HttpPost]
        public IHttpActionResult CreatePhong()
        {
            if (HttpContext.Current.Request == null || !HttpContext.Current.Request.Files.AllKeys.Any())
                return BadRequest("No files uploaded");

            try
            {
                // Lấy thông tin phòng từ form data
                var phong = new Phong
                {
                    IDLoai = Convert.ToInt32(HttpContext.Current.Request.Form["IDLoai"]),
                    IDTinhTrang = Convert.ToInt32(HttpContext.Current.Request.Form["IDTinhTrang"]),
                    Gia = Convert.ToDecimal(HttpContext.Current.Request.Form["Gia"])
                };

                if (phong.Gia <= 0)
                    return BadRequest("Giá phòng phải lớn hơn 0");

                var file = HttpContext.Current.Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    // Kiểm tra file
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    string mimeType = file.ContentType;

                    if (!allowedExtensions.Contains(extension) || !allowedMimeTypes.Contains(mimeType))
                        return BadRequest("Invalid file type");

                    if (file.ContentLength > 5 * 1024 * 1024)
                        return BadRequest("File size cannot exceed 5MB");

                    // Lưu file
                    string filename = Guid.NewGuid() + extension;
                    string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/images/"), filename);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    file.SaveAs(path);
                    phong.ImagePhong = "~/Content/images/" + filename;
                }
                else
                {
                    return BadRequest("Please upload an image for the room");
                }

                database.Phong.Add(phong);
                database.SaveChanges();

                return Ok(new
                {
                    success = true,
                    message = "Room created successfully",
                    data = new
                    {
                        phong.PhongID,
                        phong.Gia,
                        phong.IDLoai,
                        phong.IDTinhTrang,
                        phong.ImagePhong
                    }
                });
            }
            catch (Exception ex)
            {
                // Logger.Error(ex, "Error creating room");
                return InternalServerError(new Exception("Error creating room: " + ex.Message));
            }
        }


        [Route("EditPhong/{id}")]
        [HttpPut]
        public IHttpActionResult EditPhong(int id)
        {
            try
            {
                var existingPhong = database.Phong.Find(id);
                if (existingPhong == null)
                    return NotFound();

                // Cập nhật thông tin từ form data
                if (HttpContext.Current.Request.Form["Gia"] != null)
                    existingPhong.Gia = Convert.ToDecimal(HttpContext.Current.Request.Form["Gia"]);

                if (HttpContext.Current.Request.Form["IDLoai"] != null)
                    existingPhong.IDLoai = Convert.ToInt32(HttpContext.Current.Request.Form["IDLoai"]);

                if (HttpContext.Current.Request.Form["IDTinhTrang"] != null)
                    existingPhong.IDTinhTrang = Convert.ToInt32(HttpContext.Current.Request.Form["IDTinhTrang"]);

                // Validation
                if (existingPhong.Gia <= 0)
                    return BadRequest("Giá phòng phải lớn hơn 0");

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
                        if (!string.IsNullOrEmpty(existingPhong.ImagePhong))
                        {
                            var oldImagePath = HttpContext.Current.Server.MapPath(existingPhong.ImagePhong);
                            if (File.Exists(oldImagePath))
                                File.Delete(oldImagePath);
                        }

                        string filename = Path.GetFileNameWithoutExtension(file.FileName);
                        filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/images/"), filename);
                        file.SaveAs(path);
                        existingPhong.ImagePhong = "~/Content/images/" + filename;
                    }
                }

                database.Entry(existingPhong).State = EntityState.Modified;
                database.SaveChanges();

                return Ok(new
                {
                    success = true,
                    message = "Cập nhật phòng thành công",
                    data = new
                    {
                        existingPhong.PhongID,
                        existingPhong.Gia,
                        existingPhong.IDLoai,
                        existingPhong.IDTinhTrang,
                        existingPhong.ImagePhong
                    }
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Lỗi khi cập nhật phòng: " + ex.Message));
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

        [Route("GetBookingsByUser/{userId}")]
        [HttpGet]
        public IHttpActionResult GetBookingsByUser(int userId)
        {
            try
            {
                var bookings = database.DatPhong
                    .Where(d => d.NguoiDungID == userId)
                    .Include(d => d.Phong)
                    .Include(d => d.DichVuSuDung)
                    .Select(d => new
                    {
                        d.DatPhongID,
                        d.NgayDatPhong,
                        d.NgayNhanPhong,
                        d.NgayTraPhong,
                        Phong = new
                        {
                            d.Phong.Gia,
                            LoaiPhong = d.Phong.LoaiPhong.TenLoai,
                            d.ImagePhong
                        },
                        TinhTrang = d.TinhTrangPhong.TenTinhTrang
                    })
                    .ToList();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //// GET: api/phong/GetBookings - Lấy danh sách đặt phòng
        //[Route("GetBookings")]
        //[HttpGet]
        //public IHttpActionResult GetBookings(int? tinhTrangID = null)
        //{
        //    try
        //    {
        //        var query = database.DatPhong
        //            .Include(d => d.Phong)
        //            .Include(d => d.NguoiDung)
        //            .Include(d => d.DichVuSuDung)
        //            .Include(d => d.TinhTrangPhong)
        //            .AsQueryable();

        //        if (tinhTrangID.HasValue)
        //        {
        //            query = query.Where(d => d.IDTinhTrang == tinhTrangID);
        //        }

        //        var bookings = query.Select(d => new
        //        {
        //            d.DatPhongID,
        //            d.PhongID,
        //            d.NgayDatPhong,
        //            d.NgayNhanPhong,
        //            d.NgayTraPhong,
        //            d.NguoiDungID,
        //            KhachHang = new
        //            {
        //                d.NguoiDung.TenNguoiDung,
        //                d.NguoiDung.Email,
        //                d.NguoiDung.SoDienThoai
        //            },
        //            Phong = new
        //            {
        //                d.Phong.Gia,
        //                LoaiPhong = d.Phong.LoaiPhong.TenLoai,
        //                d.ImagePhong
        //            },

        //            TinhTrang = d.TinhTrangPhong.TenTinhTrang
        //        }).ToList();

        //        return Ok(bookings);
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}

        // GET: api/phong/GetBookingDetails/5 - Lấy chi tiết đặt phòng
        [Route("GetBookingDetails/{id}")]
        [HttpGet]
        public IHttpActionResult GetBookingDetails(int id)
        {
            try
            {
                var booking = database.DatPhong
                    .Include(d => d.Phong)
                    .Include(d => d.NguoiDung)
                    .Include(d => d.DichVuSuDung)
                    .Include(d => d.TinhTrangPhong)
                    .Where(d => d.DatPhongID == id)
                    .Select(d => new
                    {
                        d.DatPhongID,
                        d.PhongID,
                        d.NgayDatPhong,
                        d.NgayNhanPhong,
                        d.NgayTraPhong,
                        d.NguoiDungID,
                        KhachHang = new
                        {
                            d.NguoiDung.TenNguoiDung,
                            d.NguoiDung.Email,
                            d.NguoiDung.SoDienThoai
                        },
                        Phong = new
                        {
                            d.Phong.Gia,
                            LoaiPhong = d.Phong.LoaiPhong.TenLoai,
                            d.ImagePhong
                        },

                        TinhTrang = d.TinhTrangPhong.TenTinhTrang
                    })
                    .FirstOrDefault();

                if (booking == null)
                {
                    return NotFound();
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        // GET: api/phong/CheckRoomAvailability - Kiểm tra phòng trống
        [Route("CheckRoomAvailability")]
        [HttpGet]
        public IHttpActionResult CheckRoomAvailability(int phongId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var isAvailable = !database.DatPhong.Any(d =>
                    d.PhongID == phongId &&
                    ((startDate >= d.NgayNhanPhong && startDate <= d.NgayTraPhong) ||
                     (endDate >= d.NgayNhanPhong && endDate <= d.NgayTraPhong)));

                return Ok(new { isAvailable });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    
    // GET: api/phong/GetAllRooms
    [Route("GetAllRooms")]
        [HttpGet]
        public IHttpActionResult GetAllRooms()
        {
            try
            {
                var allRooms = database.Phong
                    .Include(p => p.LoaiPhong)
                    .Include(p => p.TinhTrangPhong)
                    .Select(p => new
                    {
                        p.PhongID,
                        p.Gia,
                        LoaiPhong = p.LoaiPhong.TenLoai.Trim(),
                        TinhTrang = p.TinhTrangPhong.TenTinhTrang.Trim(),
                        p.ImagePhong
                    })
                    .ToList();

                return Ok(allRooms);
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