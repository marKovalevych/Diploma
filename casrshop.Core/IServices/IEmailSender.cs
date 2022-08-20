using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace casrshop.Core.IServices
{
    public interface IEmailSender
    {
        public Task SendEmail(string email, string message);
    }
}
