using carshop.Infrastructure.DataAccess;
using casrshop.Core;
using casrshop.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace carshop.Infrastructure.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly CarShopContext _carShopContext;

        public CarRepository(CarShopContext carShopContext)
        {
            _carShopContext = carShopContext;
        }
        public async Task AddCarAsync(Car car)
        {
            await _carShopContext.AddAsync(car);
            await _carShopContext.SaveChangesAsync();
        }

        

        public async Task DeleteByModelAsync(string model)
        {
            var toRemove = await _carShopContext.Cars.FirstOrDefaultAsync(x => x.Model==model);
            _carShopContext.Cars.Remove(toRemove);
            await _carShopContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await _carShopContext.Cars.ToListAsync();
        }

        public async Task<Car> GetByIdAsync(int id)
        {
            return await _carShopContext.Cars.FirstOrDefaultAsync(x=>x.Id==id);
        }

        public async Task<Car> GetByModelAsync(string model)
        {
            return await _carShopContext.Cars.FirstOrDefaultAsync(x => x.Model==model);
        }

        public async Task UpdateAsync(Car car)
        {
            _carShopContext.Cars.Update(car);
            await _carShopContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Car>> GetByDateAsync(DateTime startDate, DateTime endDate)
        {
            var carsToReturn =await _carShopContext.Orders.Where(x => (startDate>x.Startdate && startDate<x.Enddate) || (endDate>x.Startdate && endDate<x.Enddate)).Select(x => x.Car_Id).ToListAsync();
            var carsAvailable = new List<Car>();
            if (carsToReturn.Count==0)
            {
                carsAvailable = _carShopContext.Cars.Select(x=>x).ToList();
            }
            else
            {
                foreach (var car in _carShopContext.Cars)
                {
                    if (carsToReturn.Contains(car.Id)==false)
                    {
                        carsAvailable.Add(car);
                    }
                }
            }

            return carsAvailable;
        }

        public async Task<bool> UpdateAsync(Car car, int id)
        {
            var toUpdate= await GetByIdAsync(id);
            if (toUpdate!=null)
            {
                toUpdate.Manufacturer=car.Manufacturer;
                toUpdate.Model=car.Model;
                toUpdate.Yearof=car.Yearof;
                toUpdate.Gearbox=car.Gearbox;
                toUpdate.Price=car.Price;

                await UpdateAsync(toUpdate);

                return true;
            }

            return false;
        }
    }
}
