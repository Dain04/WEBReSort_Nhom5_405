
using DemoDB2.Services.VisitorParttern;

namespace DemoDB2.Models.VisitorPattern
{
    public class VIPPhong : IPhong
    {
        public int PhongID { get; set; }
        public int IDLoai { get; set; } = 1; // Phòng VIP
        public int? IDTinhTrang { get; set; }
        public decimal? Gia { get; set; }

        public void Accept(IPhongVisitor visitor) => visitor.Visit(this);
    }
}
