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
    public class OrderRepository : IOrderRepository
    {
        private readonly CarShopContext _carShopContext;

        public OrderRepository(CarShopContext carShopContext)
        {
            _carShopContext=carShopContext;
        }

        public async Task CreateOrderAsync(Order order)
        {
            await _carShopContext.Orders.AddAsync(order);
            await _carShopContext.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id)
        {
            var toRemove = await _carShopContext.Orders.Where(x=>x.Id==id).FirstAsync();
            _carShopContext.Remove(toRemove);
            await _carShopContext.SaveChangesAsync();
        }

        
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _carShopContext.Orders.ToListAsync();
        }

        public async Task<Order> GetByNameAsync(string name, string lastName)
        {
            return await _carShopContext.Orders.Where(x=>x.Firstname==name && x.Lastname==lastName).FirstAsync();
             
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _carShopContext.Orders.Where(x => x.Id==id).FirstAsync();
        } 

        public async Task UpdateOrderAsync(Order order)
        {
            _carShopContext.Orders.Update(order);
            await _carShopContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Order order, int id)
        {
            var toUpdate = await GetByIdAsync(id);
            if (toUpdate!=null)
            {
                toUpdate.Firstname=order.Firstname;
                toUpdate.Lastname=order.Lastname;
                toUpdate.Email=order.Email;
                toUpdate.Car_Id=order.Car_Id;
                toUpdate.Startdate=order.Startdate;
                toUpdate.Enddate=order.Enddate;

                await UpdateOrderAsync(toUpdate);

                return true;
            }

            return false;
        }
    }
}
