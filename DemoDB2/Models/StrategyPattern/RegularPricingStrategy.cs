using DemoDB2.Models.StrategyPattern;
// Giá mặc định
namespace DemoDB2.Models.Strategies
{
    public class RegularPricingStrategy : IPricingStrategy
    {
        public decimal CalculatePrice(decimal basePrice)
        {
            return basePrice; // Giá mặc định không thay đổi
        }
    }
}
