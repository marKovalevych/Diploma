using casrshop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace casrshop.Core.Repositories
{
    public interface IAdminRepository
    {
        public Task<bool> CheckAdminAsync(string login, string password);
        
    }
}
