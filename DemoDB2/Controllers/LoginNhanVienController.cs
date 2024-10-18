using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DemoDB2.Models;

namespace DemoDB2.Controllers
{
    public class LoginNhanVienController : Controller
    {
        QLKSEntities database = new QLKSEntities();
        // GET: LoginNhanVien
        public ActionResult SelectIDChucVu()
        {
            ChucVu se_cate = new ChucVu();
            se_cate.ListChucVu = database.ChucVu.ToList<ChucVu>();
            return PartialView("SelectIDChucVu", se_cate);
        }
        public ActionResult LoginNV()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginNV(NhanVien _user)
        {
            var check = database.NhanVien.Where(s => s.Email == _user.Email && s.MatKhau == _user.MatKhau).FirstOrDefault();
            if (check == null)
            {
                ViewBag.ErrorInfo = "Sai thông tin đăng nhập";
                return View("LoginNV");
            }
            else
            {
                database.Configuration.ValidateOnSaveEnabled = false;
                Session["NameUser"] = _user.Email;
                Session["PasswordUser"] = _user.MatKhau;
                Session["NhanVienID"] = check.NhanVienID;
                Session["ChucVuID"] = check.ChucVuID; 
                return RedirectToAction("TrangChuNV", "HomeNV");
            }
        }
        public ActionResult RegisterNhanVien()
        {
            ViewBag.ListChucVu = database.ChucVu.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult RegisterNhanVien(NhanVien _user, string ConfirmPassword)
        {
            System.Diagnostics.Debug.WriteLine("RegisterNhanVien method called");
            ViewBag.ListChucVu = database.ChucVu.ToList();

            if (ModelState.IsValid)
            {
                bool isValidEmail = Regex.IsMatch(_user.Email, @"\A(?:[a-zA-Z][a-zA-Z0-9]*@(?:gmail\.com|yahoo\.com)\z)");

                if (!_user.Email.Contains("@") || !isValidEmail)
                {
                    ViewBag.ErrorRegister = "Email không hợp lệ.";
                    return View();
                }

                if (_user.MatKhau != ConfirmPassword)
                {
                    ViewBag.ErrorRegister = "Mật khẩu và xác nhận mật khẩu không khớp.";
                    return View();
                }

                var check_Email = database.NhanVien.Where(s => s.Email == _user.Email).FirstOrDefault();
                if (check_Email == null)
                {
                    database.Configuration.ValidateOnSaveEnabled = false;
                    database.NhanVien.Add(_user);
                    database.SaveChanges();
                    return RedirectToAction("LoginNV");
                }
                else
                {
                    ViewBag.ErrorRegister = "Email này đã tồn tại";
                    return View();
                }
            }
            return View();
        }
        public ActionResult LogOutNV()
        {
            Session.Abandon();
            return RedirectToAction("LoginNV", "LoginNhanVien");
        }

        public ActionResult ProfileNV()
        {
            if (Session["NameUser"] == null || Session["PasswordUser"] == null)
            {
                return RedirectToAction("LoginNV", "LoginNhanVien");
            }

            string nameUser = (string)Session["NameUser"];
            if (Session["NhanVienID"] == null)
            {
                return RedirectToAction("LoginNV", "LoginNhanVien");
            }
            int id = (int)Session["NhanVienID"];

            NhanVien user = database.NhanVien.Where(s => s.Email == nameUser && s.NhanVienID == id).FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction("LoginNV", "LoginNhanVien");
            }
            var luongMoiNhat = database.Luong
             .Where(l => l.NhanVienID == id)
             .OrderByDescending(l => l.Nam)
             .ThenByDescending(l => l.Thang)
             .FirstOrDefault();

            ViewBag.LuongMoiNhat = luongMoiNhat;

            return View(user);
        }
        public ActionResult EditNV(int id)
        {
            NhanVien user = database.NhanVien.FirstOrDefault(u => u.NhanVienID == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        public ActionResult EditNV(NhanVien model, string newPassword, string confirmNewPassword)
        {
            if (ModelState.IsValid)
            {
                var existingUser = database.NhanVien.Find(model.NhanVienID);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }
                existingUser.Ten = model.Ten;
                existingUser.DiaChi = model.DiaChi;
                existingUser.SoDienThoai = model.SoDienThoai;
                existingUser.Email = model.Email;

                // Xử lý mật khẩu
                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (newPassword == confirmNewPassword)
                    {
                        existingUser.MatKhau = newPassword;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                        return View(model);
                    }
                }
                // Nếu mật khẩu mới trống, giữ nguyên mật khẩu cũ

                try
                {
                    database.SaveChanges();
                    return RedirectToAction("ProfileNV");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng kiểm tra lại thông tin.");
                }
            }
            return View(model);
        }
        public ActionResult ViewNV()
        {
            if (Session["ChucVuID"] == null || (int)Session["ChucVuID"] == 2)
            {
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập vào trang này.";
                return View("AccessDenied");
            }
            var nhanVien = database.NhanVien.ToList();
            return View(nhanVien);
        }
        // GET: LoginNhanVien/DeleteNV/5
        public ActionResult DeleteNV(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = database.NhanVien.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }
        [HttpPost, ActionName("DeleteNV")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNVConfirmed(int id)
        {
            NhanVien nhanVien = database.NhanVien.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            database.NhanVien.Remove(nhanVien);
            database.SaveChanges();
            return RedirectToAction("ViewNV");
        }
    }
}