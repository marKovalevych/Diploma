using casrshop.Core;
using casrshop.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace carshop.Infrastructure.Services
{
    public class PriceCalculator : IPriceCalculator
    {
        public void CalculatePrice(Order order, Car car)
        {
            order.Price=car.Price*(order.Enddate-order.Startdate).TotalDays+car.Insurance;
        }
    }
}
