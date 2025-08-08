using DemoDB2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoDB2.Models.TemplatePattern
{
    public abstract class DichVuTemplate
    {
        GioHangViewModel item;
        int monAnId;
        int soLuong;
        public void XulyGioHang(GioHangViewModel item) 
        {
            AddToCart(item);
            UpdateItem(monAnId,soLuong);
        }

        public abstract void AddToCart(GioHangViewModel item);
        public abstract void UpdateItem(int monAnId, int soLuong);


    }
}