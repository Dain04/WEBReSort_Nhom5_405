using DemoDB2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DemoDB2.Controllers
{
    public class LoginUserController : Controller
    {
        QLKSEntities database = new QLKSEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginAcount(NguoiDung _user)
        {
            var check = database.NguoiDung.Where(s => s.Email == _user.Email && s.MatKhau == _user.MatKhau).FirstOrDefault();
            if (check == null)
            {
                ViewBag.ErrorInfo = "Sai thông tin đăng nhập";
                return View("Index");
            }
            else
            {
                database.Configuration.ValidateOnSaveEnabled = false;
                Session["NameUser"] = _user.Email;
                Session["PasswordUser"] = _user.MatKhau;
                Session["ID"] = check.NguoiDungID;
                return RedirectToAction("TrangChu", "Home");
            }
        }

        public ActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterUser(NguoiDung _user, string ConfirmPassword)
        {
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

                var check_Email = database.NguoiDung.Where(s => s.Email == _user.Email).FirstOrDefault();
                if (check_Email == null)
                {
                    database.Configuration.ValidateOnSaveEnabled = false;
                    database.NguoiDung.Add(_user);
                    database.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorRegister = "Email này đã tồn tại";
                    return View();
                }
            }
            return View();
        }

        public ActionResult LogOutUser()
        {
            Session.Abandon();
            return RedirectToAction("Index", "LoginUser");
        }

        public new ActionResult Profile()
        {
            if (Session["NameUser"] == null || Session["PasswordUser"] == null)
            {
                return RedirectToAction("Index", "LoginUser");
            }

            string nameUser = (string)Session["NameUser"];
            int id = (int)Session["ID"];

            NguoiDung user = database.NguoiDung.Where(s => s.Email == nameUser && s.NguoiDungID == id).FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction("Index", "LoginUser");
            }
            return View(user);
        }

        public ActionResult Edit(int id)
        {
            NguoiDung user = database.NguoiDung.FirstOrDefault(u => u.NguoiDungID == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(NguoiDung model, string newPassword, string confirmNewPassword)
        {
            if (ModelState.IsValid)
            {
                var existingUser = database.NguoiDung.Find(model.NguoiDungID);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật các trường thông tin khác
                existingUser.TenNguoiDung = model.TenNguoiDung;
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
                    return RedirectToAction("Profile");
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
                    // Thêm thông báo lỗi vào ModelState
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng kiểm tra lại thông tin.");
                }
            }
            return View(model);
        }
    }
}
