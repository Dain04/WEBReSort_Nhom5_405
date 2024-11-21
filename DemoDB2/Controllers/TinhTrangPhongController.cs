using DemoDB2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoDB2.Controllers
{
    public class TinhTrangPhongController : Controller
    {
        QLKSEntities database = new QLKSEntities();
        public PartialViewResult ListTinhTrang()
        {
            var cateList = database.LoaiPhong.ToList();
            return PartialView(cateList);
        }
        public ActionResult CreateTinhTrangP()
        {
            List<TinhTrangPhong> list = database.TinhTrangPhong.ToList();
            ViewBag.listCategory = new SelectList(list, "ID Tình Trạng Phòng", "Tên Tình Trạng Phòng", "");
            Phong pro = new Phong();
            return View();
        }
        [HttpPost]
        public ActionResult CreateTinhTrangP(TinhTrangPhong status)
        {
            try
            {
                database.TinhTrangPhong.Add(status);
                database.SaveChanges();
                return RedirectToAction("ViewListTinhTrangP");
            }
            catch
            {
                return Content("Error Create New");
            }
        }


        public ActionResult EditTinhTrangPhong(int id)
        {
            return View(database.TinhTrangPhong.Where(s => s.IDTinhTrang == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditTinhTrangPhong(int id, TinhTrangPhong status)
        {
            database.Entry(status).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            return RedirectToAction("ViewListTinhTrangP");
        }
        public ActionResult DeleteTinhTrangPhong(int id)
        {
            return View(database.TinhTrangPhong.Where(s => s.IDTinhTrang == id).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult DeleteTinhTrangPhong(int id, TinhTrangPhong status)
        {
            try
            {
                status = database.TinhTrangPhong.Where(s => s.IDTinhTrang == id).FirstOrDefault();
                database.TinhTrangPhong.Remove(status);
                database.SaveChanges();
                return RedirectToAction("ViewListTinhTrangP");
            }
            catch
            {
                return Content("This data is using in other table, Error Delete!");
            }
        }
        public ActionResult ViewListTinhTrangP(string _name)
        {
            if (_name == null)
                return View(database.TinhTrangPhong.ToList());
            else
                return View(database.TinhTrangPhong.Where(s => s.TenTinhTrang.Contains(_name)).ToList());
        }
    }
}