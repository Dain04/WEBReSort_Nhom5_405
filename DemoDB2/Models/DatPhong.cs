
namespace DemoDB2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DatPhong
    {
        public int DatPhongID { get; set; }
        public Nullable<int> PhongID { get; set; }
        public Nullable<System.DateTime> NgayDatPhong { get; set; }
        public Nullable<System.DateTime> NgayNhanPhong { get; set; }
        public Nullable<System.DateTime> NgayTraPhong { get; set; }
        public Nullable<int> NguoiDungID { get; set; }
        public Nullable<int> IDDichVuSuDung { get; set; }
        public Nullable<int> IDTinhTrang { get; set; }
        public string ImagePhong { get; set; }
    
        public virtual DichVuSuDung DichVuSuDung { get; set; }
        public virtual TinhTrangPhong TinhTrangPhong { get; set; }
        public virtual NguoiDung NguoiDung { get; set; }
        public virtual Phong Phong { get; set; }
    }
}
