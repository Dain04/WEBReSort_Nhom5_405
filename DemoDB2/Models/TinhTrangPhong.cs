//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DemoDB2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TinhTrangPhong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TinhTrangPhong()
        {
            this.DatPhong = new HashSet<DatPhong>();
            this.Phong = new HashSet<Phong>();
        }
    
        public int IDTinhTrang { get; set; }
        public string TenTinhTrang { get; set; }
        public List<TinhTrangPhong> ListTinhTrang { get; internal set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatPhong> DatPhong { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Phong> Phong { get; set; }
    }
}