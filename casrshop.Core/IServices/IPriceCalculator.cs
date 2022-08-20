using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace casrshop.Core.IServices
{
    public interface IPriceCalculator
    {
        public void CalculatePrice(Order order, Car car);
    }
}
