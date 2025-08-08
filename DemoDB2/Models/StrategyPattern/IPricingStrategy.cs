using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDB2.Models.StrategyPattern
{
    public interface IPricingStrategy
    {
        decimal CalculatePrice(decimal basePrice);
    }
}
