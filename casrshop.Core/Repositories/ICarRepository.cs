using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace casrshop.Core.Repositories
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetAllAsync();
        Task<IEnumerable<Car>> GetByDateAsync(DateTime startDate, DateTime endDate);
        Task<Car> GetByIdAsync(int id);
        Task<Car> GetByModelAsync(string model);
        Task AddCarAsync(Car car);
        Task DeleteByModelAsync(string model);
        Task<bool> UpdateAsync(Car car, int id);
    }
}
