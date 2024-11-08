

namespace DemoDB2.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    public partial class Phong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Phong()
        {
            this.DatPhong = new HashSet<DatPhong>();
            this.HoaDon = new HashSet<HoaDon>();
        }
    
        public int PhongID { get; set; }
        public Nullable<int> IDTinhTrang { get; set; }
        public Nullable<int> IDLoai { get; set; }
        public Nullable<decimal> Gia { get; set; }
        public string ImagePhong { get; set; }
        public HttpPostedFileBase UploadImage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatPhong> DatPhong { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HoaDon> HoaDon { get; set; }
        public virtual LoaiPhong LoaiPhong { get; set; }
        public virtual TinhTrangPhong TinhTrangPhong { get; set; }
    }
}
