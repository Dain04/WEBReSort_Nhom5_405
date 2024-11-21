using DemoDB2.Models;
using System.Net;
using System.Web.Mvc;
using System;

[Authorize]
public class DatPhongController : Controller
{
    private QLKSEntities db = new QLKSEntities();

    // GET: DatPhong/Create
    public ActionResult Create(int? phongId)
    {
        if (phongId == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        var phong = db.Phong.Find(phongId);
        if (phong == null)
        {
            return HttpNotFound();
        }

        // Lấy thông tin người dùng từ Session
        int? userId = Session["ID"] as int?;
        if (!userId.HasValue)
        {
            return RedirectToAction("Index", "LoginUser");
        }

        var currentUser = db.NguoiDung.Find(userId.Value);
        if (currentUser == null)
        {
            return HttpNotFound();
        }

        var datPhong = new DatPhong
        {
            PhongID = phong.PhongID,
            NguoiDungID = currentUser.NguoiDungID,
            NgayDatPhong = DateTime.Now
        };

        ViewBag.TenNguoiDung = currentUser.TenNguoiDung;
        ViewBag.TenPhong = phong.LoaiPhong;

        return View(datPhong);
    }

    // POST: DatPhong/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "PhongID,NgayNhanPhong,NgayTraPhong,NguoiDungID")] DatPhong datPhong)
    {
        if (ModelState.IsValid)
        {
            int? userId = Session["ID"] as int?;
            if (!userId.HasValue)
            {
                return RedirectToAction("Index", "LoginUser");
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Gán NguoiDungID từ session
                    datPhong.NguoiDungID = userId.Value;
                    datPhong.NgayDatPhong = DateTime.Now;

                    db.DatPhong.Add(datPhong);

                    // Cập nhật TinhTrang của Phòng
                    var phongToUpdate = db.Phong.Find(datPhong.PhongID);
                    if (phongToUpdate != null)
                    {
                        phongToUpdate.TinhTrangPhong.IDTinhTrang = 4; // ID 4 đại diện cho "Đã đặt"
                    }

                    db.SaveChanges();
                    transaction.Commit();
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Có lỗi xảy ra trong quá trình đặt phòng.");
                }
            }
        }

        // Nếu có lỗi, lấy lại thông tin để hiển thị
        var currentUser = db.NguoiDung.Find(datPhong.NguoiDungID);
        var phong = db.Phong.Find(datPhong.PhongID);
        ViewBag.TenNguoiDung = currentUser?.TenNguoiDung;
        ViewBag.TenPhong = phong?.LoaiPhong;

        return View(datPhong);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}
