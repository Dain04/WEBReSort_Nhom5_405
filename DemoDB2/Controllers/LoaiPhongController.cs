using System;
using DemoDB2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoDB2.Models.VisitorPattern;

namespace DemoDB2.Controllers
{
    public class LoaiPhongController : Controller
    {
        QLKSEntities database = new QLKSEntities();
        // GET: LoaiPhong
         
        public PartialViewResult ListLoaiPhong()
        {
            var cateList = database.LoaiPhong.ToList();
            return PartialView(cateList);
        }
        public ActionResult CreateTinhTrangP()
        {
            List<TinhTrangPhong> list = database.TinhTrangPhong.ToList();
            ViewBag.listCategory = new SelectList(list, "IDTìnhTrạngPhòng", "TênTìnhTrạngPhòng", "");

            // Dùng một lớp con thay vì IPhong
            IPhong pro = new StandardPhong(); // hoặc new VIPPhong();

            return View(pro);
        }

        [HttpPost]
        public ActionResult CreateLoaiPhong(LoaiPhong loai)
        {
            try
            {
                database.LoaiPhong.Add(loai);
                database.SaveChanges();
                return RedirectToAction("ViewLoaiPhong");
            }
            catch
            {
                return Content("Error Create New");
            }
        }

       
        public ActionResult EditLoaiPhong(int id)
        {
            return View(database.LoaiPhong.Where(s => s.IDLoai == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditLoaiPhong(int id, LoaiPhong loai)
        {
            database.Entry(loai).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            return RedirectToAction("ViewLoaiPhong");
        }
        public ActionResult DeleteLoaiPhong(int id)
        {
            return View(database.LoaiPhong.Where(s => s.IDLoai == id).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult DeleteLoaiPhong(int id, LoaiPhong loai)
        {
            try
            {
                loai = database.LoaiPhong.Where(s => s.IDLoai == id).FirstOrDefault();
                database.LoaiPhong.Remove(loai);
                database.SaveChanges();
                return RedirectToAction("ViewLoaiPhong");
            }
            catch
            {
                return Content("This data is using in other table, Error Delete!");
            }
        }
        public ActionResult ViewLoaiPhong(string _name)
        {
            if (_name == null)
                return View(database.LoaiPhong.ToList());
            else
                return View(database.LoaiPhong.Where(s => s.TenLoai.Contains(_name)).ToList());
        }
       
    }
}