using System.Collections.Generic;

namespace DemoDB2.ViewModels
{
    public class ChiTietHoaDonViewModel
    {
        public List<GioHangViewModel> MonAn { get; set; }
        public decimal TongTien { get; set; }
        public string QRCodeData { get; set; }

    }
}