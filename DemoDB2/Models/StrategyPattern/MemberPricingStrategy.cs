using DemoDB2.Models.StrategyPattern;
//giảm giá cho thành viên
namespace DemoDB2.Models.Strategies
{
    public class MemberPricingStrategy : IPricingStrategy
    {
        public decimal CalculatePrice(decimal basePrice)
        {
            return basePrice * 0.9m; // Giảm 10% cho thành viên
        }
    }
}
