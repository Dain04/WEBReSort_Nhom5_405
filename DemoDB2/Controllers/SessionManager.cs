using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoDB2.Controllers
{
    public static class SessionManager
    {

        // Keys cho User
        public const string USER_ID = "UserID";
        public const string USER_EMAIL = "User_Email";
        public const string USER_PASSWORD = "UserPassword";
        public const string USER_PROFILE_IMAGE = "ProfileImage";

        // Keys cho NhanVien
        public const string NV_EMAIL = "NV_Email";
        public const string NV_PASSWORD = "PasswordNV";
        public const string NV_ID = "NhanVienID";
        public const string NV_CHUCVU_ID = "ChucVuID";
        public const string NV_TENCHUCVU = "TenChucVu";

        public static void ClearUserSession()
        {
            var currentContext = HttpContext.Current;
            if (currentContext != null)
            {
                System.Diagnostics.Debug.WriteLine("Clearing User Session");
                currentContext.Session.Remove(USER_ID);
                currentContext.Session.Remove(USER_EMAIL);
                currentContext.Session.Remove(USER_PASSWORD);
                currentContext.Session.Remove(USER_PROFILE_IMAGE);
                System.Diagnostics.Debug.WriteLine("User Session Cleared");
            }
        }

        public static void ClearNhanVienSession()
        {
            var currentContext = HttpContext.Current;
            if (currentContext != null)
            {
                System.Diagnostics.Debug.WriteLine("Clearing NhanVien Session");
                currentContext.Session.Remove(NV_EMAIL);
                currentContext.Session.Remove(NV_PASSWORD);
                currentContext.Session.Remove(NV_ID);
                currentContext.Session.Remove(NV_CHUCVU_ID);
                currentContext.Session.Remove(NV_TENCHUCVU);
                System.Diagnostics.Debug.WriteLine("NhanVien Session Cleared");
            }
        }

        public static bool IsUserLoggedIn()
        {
            var context = HttpContext.Current;
            var isLoggedIn = context?.Session[USER_EMAIL] != null &&
                             context?.Session[USER_ID] != null;
            System.Diagnostics.Debug.WriteLine($"IsUserLoggedIn: {isLoggedIn}");
            return isLoggedIn;
        }


        public static bool IsNhanVienLoggedIn()
        {
            var context = HttpContext.Current;
            var isLoggedIn = context?.Session[NV_EMAIL] != null;
            System.Diagnostics.Debug.WriteLine($"IsNhanVienLoggedIn: {isLoggedIn}");
            return isLoggedIn;
        }
    }

}