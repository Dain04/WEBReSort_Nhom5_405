using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DemoDB2.Models;
using PagedList;
using static DemoDB2.Models.Phong;

namespace DemoDB2.Controllers
{
    public class PhongController : Controller
    {
        QLKSEntities database = new QLKSEntities();

        public ActionResult SelectLoai()
        {
            LoaiPhong se_cate = new LoaiPhong();
            se_cate.ListLoai = database.LoaiPhong.ToList<LoaiPhong>();
            return PartialView("SelectLoai", se_cate);
        }

        public ActionResult CreatePhong()
        {
            ViewBag.LoaiPhongList = new SelectList(database.LoaiPhong, "TenLoai", "TenLoai");
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

                ViewBag.LoaiPhongList = new SelectList(database.LoaiPhong, "TenLoai", "TenLoai");
                return View(pro);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                ViewBag.LoaiPhongList = new SelectList(database.LoaiPhong, "TenLoai", "TenLoai");
                return View(pro);
            }
        }

        public ActionResult ViewPhong(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var phongs = database.Phong.OrderBy(p => p.PhongID).ToList();
            var pagedPhongs = phongs.ToPagedList(pageNumber, pageSize);
            return View(pagedPhongs);
        }
        public ActionResult ViewPhongKH(int? page)
        {
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            var model = database.Phong.OrderBy(p => p.PhongID).ToPagedList(pageNumber, pageSize);
            return View(model);
        }
        public ActionResult EDITPhong(int id)
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


            ViewBag.LoaiPhongList = new SelectList(database.LoaiPhong, "TenLoai", "TenLoai", phong.LoaiP);

            return View(phong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EDITPhong(Phong phong)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (phong.UploadImage != null && phong.UploadImage.ContentLength > 0)
                    {
                        string filename = Path.GetFileNameWithoutExtension(phong.UploadImage.FileName);
                        string extension = Path.GetExtension(phong.UploadImage.FileName);
                        filename = filename + extension;
                        string path = Path.Combine(Server.MapPath("~/Content/images/"), filename);
                        phong.ImagePhong = "~/Content/images/" + filename;
                        phong.UploadImage.SaveAs(path);
                    }

                    database.Entry(phong).State = System.Data.Entity.EntityState.Modified;
                    database.SaveChanges();
                    return RedirectToAction("ViewPhong");
                }


                ViewBag.LoaiPhongList = new SelectList(database.LoaiPhong, "TenLoai", "TenLoai", phong.LoaiP);
                return View(phong);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);

                ViewBag.LoaiPhongList = new SelectList(database.LoaiPhong, "TenLoai", "TenLoai", phong.LoaiP);
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
                    phong.TinhTrang = false; // Đặt thành đã đặt

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
                }

                database.SaveChanges();

                TempData["SuccessMessage"] = "Đặt phòng thành công. Hóa đơn đã được tạo.";
                return RedirectToAction("ViewPhongKH");
            }

            return View(datPhong);
        }
    }
}
