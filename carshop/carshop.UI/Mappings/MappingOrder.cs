using AutoMapper;
using carshop.carshop.UI.Models;
using casrshop.Core;

namespace carshop.carshop.UI.Mappings
{
    public class MappingOrder: Profile
    {
         public MappingOrder()
        {
            CreateMap<Order, OrderModel>();
            CreateMap<OrderModel, Order>();
        }
    }
}
