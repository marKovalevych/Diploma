using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using casrshop.Core.IServices;
using System.Net.Mail;
using casrshop.Core;
using System.Net;

namespace carshop.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmail(string messageToSend, string email)
        {
            string from = "carrentservicelviv@gmail.com";
            string password = "xjoxzxmcehgyzabf";
            var to=new MailAddress(email);
            MailMessage msg=new MailMessage();
            msg.Subject="Your order";
            msg.Body=messageToSend;
            msg.From=new MailAddress(from);
            msg.To.Add(to);

            SmtpClient smtp = new SmtpClient();
            smtp.Host="smtp.gmail.com";
            smtp.Port=587;
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            NetworkCredential nc = new NetworkCredential(from, password);
            smtp.DeliveryMethod=SmtpDeliveryMethod.Network;
            smtp.Credentials=nc;

            await smtp.SendMailAsync(msg);
        }
    }
}
