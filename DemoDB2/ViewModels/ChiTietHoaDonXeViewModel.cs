using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoDB2.ViewModels
{
    public class ChiTietHoaDonXeViewModel
    {
        public List<GioHangXeViewModel> Xe { get; set; }
        public decimal TongTien { get; set; }
    }
}