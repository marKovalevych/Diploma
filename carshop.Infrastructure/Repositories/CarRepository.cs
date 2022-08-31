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

        

        public async Task DeleteByIdAsync(int id)
        {
            var toRemove = await _carShopContext.Cars.FirstOrDefaultAsync(x => x.Id == id);
            _carShopContext.Cars.Remove(toRemove);
            await _carShopContext.SaveChangesAsync();
        }

        public async Task<List<Car>> GetAllAsync()
        {
            return await _carShopContext.Cars.ToListAsync();
        }

        public async Task<Car> GetByIdAsync(int id)
        {
            return await _carShopContext.Cars.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Car> GetByModelAsync(string model)
        {
            return await _carShopContext.Cars.FirstOrDefaultAsync(x => x.Model == model);
        }

        public async Task UpdateAsync(Car car)
        {
            _carShopContext.Cars.Update(car);
            await _carShopContext.SaveChangesAsync();
        }

        public async Task<List<Car>> GetByDateAsync(DateTime startDate, DateTime endDate)
        {
            var carNotAvailable = _carShopContext.Orders
                .Include(x => x.Car)
                .Where(x => (x.Startdate <= startDate && startDate <= x.Enddate) || (x.Startdate <= endDate && endDate <= x.Enddate))
                .Select(x => x.Car)
                .ToHashSet();

            return await _carShopContext.Cars.Where(x => !carNotAvailable.Contains(x)).ToListAsync();

        }
    }
}
