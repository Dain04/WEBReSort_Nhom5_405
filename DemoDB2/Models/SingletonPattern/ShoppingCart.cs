using DemoDB2.Models.TemplatePattern;
using DemoDB2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoDB2.Models.SingletonPattern
{
    public class ShoppingCart : DichVuTemplate
    {
        //Singleton Pattern
        private static ShoppingCart _instance; 
        private static readonly object _lock = new object();
        //public List<GioHangViewModel> Items { get; private set; }
        private Dictionary<string, List<GioHangViewModel>> _userCarts = new Dictionary<string, List<GioHangViewModel>>();

        // lấy key của người dùng hiện tại
        private string GetUserKey()
        {
            return HttpContext.Current.Session.SessionID; 
        }

        //lấy giỏ hàng của người dùng tương ứng với key
        private List<GioHangViewModel> GetCartForCurrentUser()
        {
            var key = GetUserKey();
            if (!_userCarts.ContainsKey(key))
            {
                _userCarts[key] = new List<GioHangViewModel>();
            }
            return _userCarts[key];
        }

        private Dictionary<string, List<GioHangViewModel>> userCarts;

        private ShoppingCart()
        {
            userCarts = new Dictionary<string, List<GioHangViewModel>>();
        }

        // Phương thức để lấy thể hiện duy nhất
        public static ShoppingCart Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ShoppingCart();
                    }
                    return _instance;
                }
            }
        }

        private List<GioHangViewModel> GetUserCart(string userKey)
        {
            if (!userCarts.ContainsKey(userKey))
            {
                userCarts[userKey] = new List<GioHangViewModel>();
            }
            return userCarts[userKey];
        }

        // Các phương thức để thêm, cập nhật, xóa món ăn trong giỏ hàng
        public override void AddToCart(GioHangViewModel item)
        {
            var cart = GetCartForCurrentUser();
            var existing = cart.FirstOrDefault(i => i.MonAnId == item.MonAnId);
            if (existing != null)
                existing.SoLuong += item.SoLuong;
            else
                cart.Add(item);
        }

        public override void UpdateItem(int monAnId, int soLuong)
        {
            var cart = GetCartForCurrentUser();
            var item = cart.FirstOrDefault(i => i.MonAnId == monAnId);
            if (item != null)
                item.SoLuong = soLuong;
        }

        public void RemoveItem(int monAnId)
        {
            var cart = GetCartForCurrentUser();
            var item = cart.FirstOrDefault(i => i.MonAnId == monAnId);
            if (item != null)
                cart.Remove(item);
        }

        public void XoaGioHang()
        {
            var key = GetUserKey();
            if (_userCarts.ContainsKey(key))
                _userCarts.Remove(key);
        }
        public List<GioHangViewModel> GetItems()
        {
            return GetCartForCurrentUser();
        }
    }
}