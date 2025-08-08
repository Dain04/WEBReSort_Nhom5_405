using DemoDB2.Models.StrategyPattern;

namespace DemoDB2.Models.Strategies
{
    public class RoomBookingContext
    {
        private IPricingStrategy _pricingStrategy;

        public RoomBookingContext(IPricingStrategy pricingStrategy)
        {
            _pricingStrategy = pricingStrategy;
        }

        public decimal GetFinalPrice(decimal basePrice)
        {
            return _pricingStrategy.CalculatePrice(basePrice);
        }
    }
}
