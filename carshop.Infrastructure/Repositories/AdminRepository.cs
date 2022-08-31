using carshop.Infrastructure.DataAccess;
using casrshop.Core.Entities;
using casrshop.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace carshop.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly CarShopContext _context;

        public AdminRepository(CarShopContext context)
        {
            _context=context;
        }

        public async Task<bool> CheckAdminAsync(string login, string password)
        {
            var admin = await _context.Admins.Where(x => x.Login==login && x.Password==password).FirstOrDefaultAsync();

            if (admin!=null)
            {
                return true;
            }

            return false;
        }

    }
}
