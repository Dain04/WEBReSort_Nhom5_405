using DemoDB2.Models.StrategyPattern;

namespace DemoDB2.Models.Strategies
{
    public class HolidayPricingStrategy : IPricingStrategy
    {
        public decimal CalculatePrice(decimal basePrice)
        {
            return basePrice * 1.2m; // Tăng 20% vào ngày lễ
        }
    }
}
