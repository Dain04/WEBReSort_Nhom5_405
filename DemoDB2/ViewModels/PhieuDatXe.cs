using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoDB2.ViewModels
{
    public class PhieuDatXe
    {
        public int Id { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public List<GioHangXeViewModel> Xe { get; set; }
        public decimal TongTien { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public DateTime NgayDat { get; set; }
        public string HieuXe { get; set; }
        public string BienSoXe { get; set; }
        public string TaiXe { get; set; }
    }
}