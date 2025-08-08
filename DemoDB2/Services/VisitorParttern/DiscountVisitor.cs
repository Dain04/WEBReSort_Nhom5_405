using DemoDB2.Models.VisitorPattern;

namespace DemoDB2.Services.VisitorParttern
{
    public class DiscountVisitor : IPhongVisitor
    {
        public decimal? DiscountedPrice { get; private set; }

        public void Visit(StandardPhong phong)
        {
            DiscountedPrice = phong.Gia * 0.9m; // Giảm 10%
        }

        public void Visit(VIPPhong phong)
        {
            DiscountedPrice = phong.Gia * 0.8m; // Giảm 20%
        }
    }
}
