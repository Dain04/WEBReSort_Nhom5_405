
using DemoDB2.Services.VisitorParttern;

namespace DemoDB2.Models.VisitorPattern
{
    public interface IPhong
    {
        int PhongID { get; set; } 
        int IDLoai { get; set; }
        int? IDTinhTrang { get; set; }
        decimal? Gia { get; set; }

        void Accept(IPhongVisitor visitor);
    }
}
