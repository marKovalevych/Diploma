using AutoMapper;
using carshop.carshop.UI.Models;
using casrshop.Core;

namespace carshop.carshop.UI.Mappings
{
    public class MappingCar : Profile
    {
        public MappingCar()
        {
            CreateMap<CarModel, Car>();
            CreateMap<Car, CarModel>();
        }
    }
}
