using DemoDB2.Models;
using Microsoft.Owin.Security;
using System;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            var check = database.NguoiDung.FirstOrDefault(s => s.Email == _user.Email && s.MatKhau == _user.MatKhau);
            if (check == null)
            {
                ViewBag.ErrorInfo = "Sai thông tin đăng nhập";
                return View("Index");
            }

            System.Diagnostics.Debug.WriteLine($"User logging in: {_user.Email}");

            // Set Session với kiểm tra null
            database.Configuration.ValidateOnSaveEnabled = false;
            if (check.NguoiDungID != 0) // Kiểm tra ID hợp lệ
            {
                Session[SessionManager.USER_ID] = check.NguoiDungID;
                Session[SessionManager.USER_EMAIL] = _user.Email;
                Session[SessionManager.USER_PASSWORD] = check.MatKhau;
                Session[SessionManager.USER_PROFILE_IMAGE] = check.ImageUser ?? "/Content/Images/default-avatar.png";

                // Debug để kiểm tra
                System.Diagnostics.Debug.WriteLine($"Set USER_ID in Session: {check.NguoiDungID}");
                System.Diagnostics.Debug.WriteLine($"Set USER_EMAIL in Session: {_user.Email}");
            }

            return RedirectToAction("TrangChu", "Home");
        }
        public ActionResult LogOutUser()
        {
            System.Diagnostics.Debug.WriteLine("User logging out");
            if (SessionManager.IsUserLoggedIn())
            {
                SessionManager.ClearUserSession();
            }
            return RedirectToAction("Index", "LoginUser");
        }
        public new ActionResult Profile()
        {
            // Kiểm tra đăng nhập
            if (!SessionManager.IsUserLoggedIn())
            {
                return RedirectToAction("Index", "LoginUser");
            }

            // Lấy thông tin từ Session với null checking
            var userEmail = Session[SessionManager.USER_EMAIL]?.ToString();
            var userId = Session[SessionManager.USER_ID];

            // Kiểm tra null cho cả email và id
            if (string.IsNullOrEmpty(userEmail) || userId == null)
            {
                return RedirectToAction("Index", "LoginUser");
            }

            // Convert userId sang int sau khi đã kiểm tra null
            int id = Convert.ToInt32(userId);

            // Tìm user trong database
            NguoiDung user = database.NguoiDung
                .Where(s => s.Email == userEmail && s.NguoiDungID == id)
                .FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction("Index", "LoginUser");
            }

            return View(user);
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
                if (_user == null || string.IsNullOrWhiteSpace(_user.TenNguoiDung)|| _user.TenNguoiDung.Length>30)
                {
                    ViewBag.ErrorRegister = "Tên người dùng không được để trống hoặc phải có nhiều hơn 30 ký tự";
                    return View(_user);
                }
              
                if (string.IsNullOrWhiteSpace(_user.Email))
                {
                    ViewBag.ErrorRegister = "Hãy nhập email của bạn vào";
                    return View(_user);
                }
                bool isValidEmail = Regex.IsMatch(_user.Email, @"\A(?:[a-zA-Z][a-zA-Z0-9]*@(?:gmail\.com|yahoo\.com)\z)");
                if (!_user.Email.Contains("@") || !isValidEmail)
                {
                    ViewBag.ErrorRegister = "Email không hợp lệ.";
                    return View(_user);
                }
               
                if (string.IsNullOrWhiteSpace(_user.MatKhau)||_user.MatKhau.Length < 6)
                {
                    ViewBag.ErrorRegister = "Mật khẩu phải có ít nhât 6 ký tự";
                    return View(_user);
                }
                if (_user.MatKhau != ConfirmPassword)
                {
                    ViewBag.ErrorRegister = "Mật khẩu và xác nhận mật khẩu không khớp.";
                    return View(_user);
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
                    return View(_user);
                }
            }
            return View(_user);
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
        public ActionResult Edit(NguoiDung model, HttpPostedFileBase UploadImage, string newPassword, string confirmNewPassword)
        {
            if (ModelState.IsValid)
            {
                var existingUser = database.NguoiDung.Find(model.NguoiDungID);
                if (existingUser == null)
                {
                    return HttpNotFound();
                }

                existingUser.TenNguoiDung = model.TenNguoiDung;
                existingUser.DiaChi = model.DiaChi;
                existingUser.SoDienThoai = model.SoDienThoai;

                // Kiểm tra tính hợp lệ của email mới
                bool isValidEmail = Regex.IsMatch(model.Email, @"\A(?:[a-zA-Z][a-zA-Z0-9]*@(?:gmail\.com|yahoo\.com)\z)");
                if (isValidEmail)
                {
                    existingUser.Email = model.Email;
                }
                else
                {
                    ModelState.AddModelError("Email", "Email không hợp lệ. Email cũ sẽ được giữ nguyên.");
                    model.Email = existingUser.Email; // Giữ nguyên email cũ trong model để hiển thị lại trong view
                }

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

                if (!string.IsNullOrEmpty(model.SoDienThoai))
                {
                    if (!Regex.IsMatch(model.SoDienThoai, @"^\d{10}$"))
                    {
                        ModelState.AddModelError("SoDienThoai", "Số điện thoại phải có 10 số");
                        return View(model);
                    }
                }

                // Xử lý upload ảnh
                if (UploadImage != null && UploadImage.ContentLength > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(UploadImage.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("UploadImage", "Chỉ hỗ trợ các định dạng hình ảnh: JPEG, PNG.");
                        return View(model);
                    }
                    const int maxSize = 2 * 1024 * 1024; // 2 MB
                    if (UploadImage.ContentLength > maxSize)
                    {
                        ModelState.AddModelError("UploadImage", "Kích thước tệp không được vượt quá 2 MB.");
                        return View(model);
                    }
                    var fileName = Path.GetFileName(UploadImage.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    UploadImage.SaveAs(path);
                    existingUser.ImageUser = "~/Content/Images/" + fileName;
                    Session["ProfileImage"] = existingUser.ImageUser;
                }

                if (ModelState.IsValid) // Kiểm tra lại ModelState sau khi thực hiện tất cả các validation
                {
                    try
                    {
                        database.SaveChanges();
                        return RedirectToAction("Profile");
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            }
                        }
                        ModelState.AddModelError("", "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng kiểm tra lại thông tin.");
                    }
                }
            }
            return View(model);
        }
        public ActionResult ExternalLogin(string provider)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "LoginUser"));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Index");
            }

            // Kiểm tra xem người dùng đã tồn tại trong cơ sở dữ liệu chưa
            var user = database.NguoiDung.FirstOrDefault(u => u.Email == loginInfo.Email);
            if (user == null)
            {
                // Tạo người dùng mới nếu chưa tồn tại
                user = new NguoiDung
                {
                    Email = loginInfo.Email,
                    TenNguoiDung = loginInfo.DefaultUserName ?? "Unknown",
                    MatKhau = "123", // Đặt một mật khẩu mặc định
                    ImageUser = "/Content/Images/default-avatar.png" // Thêm ảnh mặc định
                };

                database.NguoiDung.Add(user);
                try
                {
                    await database.SaveChangesAsync();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        }
                    }
                    return RedirectToAction("Error", "Home");
                }
            }

            // Đăng nhập người dùng
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, loginInfo.ExternalIdentity);

            // Thiết lập các biến session sử dụng SessionManager
            Session[SessionManager.USER_ID] = user.NguoiDungID;
            Session[SessionManager.USER_EMAIL] = user.Email;
            Session[SessionManager.USER_PASSWORD] = user.MatKhau;
            Session[SessionManager.USER_PROFILE_IMAGE] = user.ImageUser ?? "/Content/Images/default-avatar.png";

            // Debug logging
            System.Diagnostics.Debug.WriteLine($"Google login - Set USER_ID in Session: {user.NguoiDungID}");
            System.Diagnostics.Debug.WriteLine($"Google login - Set USER_EMAIL in Session: {user.Email}");

            return RedirectToAction("TrangChu", "Home");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
          
          
          
        }
    }
}
