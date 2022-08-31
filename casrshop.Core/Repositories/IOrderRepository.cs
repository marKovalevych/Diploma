using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace casrshop.Core.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByNameAsync(string name, string lastName);
        Task<Order> GetByIdAsync(int id);
        Task<ICollection<Order>> GetOrdersByCar(Car car);
        Task CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int id);
        Task<bool> UpdateAsync(Order order, int id);
    }
}
