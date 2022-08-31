using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace casrshop.Core.IServices
{
    public interface IMessageCreator
    {
        public string MessageCreate(Order order, Car car);
        public string MessageEdit(Order order, Car car);
        public string MessageDelete(Order order);
        public string MessageToDeleteCarByAdmin(Order order, Car car);
    }
}
