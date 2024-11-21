

namespace DemoDB2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class HoaDon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HoaDon()
        {
            this.DichVuSuDung = new HashSet<DichVuSuDung>();
        }
    
        public int HoaDonID { get; set; }
        public Nullable<int> KhachHangID { get; set; }
        public Nullable<int> PhongID { get; set; }
        public Nullable<System.DateTime> NgayNhanPhong { get; set; }
        public Nullable<System.DateTime> NgayTraPhong { get; set; }
        public Nullable<decimal> TongTien { get; set; }
        public Nullable<int> IDDichVuSuDung { get; set; }
        public Nullable<int> NguoiDungID { get; set; }
        public Nullable<int> NhanVienID { get; set; }
        public string TrangThaiThanhToan { get; set; }


        public long OrderVnPayID { get; set; }
        public string Code { get; set; }
        public string TenNguoiDung { get; set; }
        public DateTime NgayTaoHD { get; set; }
        public DateTime NgayChinhSua { get; set; }



        public bool IsPaid => TrangThaiThanhToan == "Đã thanh toán";


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DichVuSuDung> DichVuSuDung { get; set; }
        public virtual NguoiDung NguoiDung { get; set; }
        public virtual Phong Phong { get; set; }
    }
}
