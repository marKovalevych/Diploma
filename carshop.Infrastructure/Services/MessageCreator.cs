using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using casrshop.Core;
using casrshop.Core.IServices;

namespace carshop.Infrastructure.Services
{
    public class MessageCreator : IMessageCreator
    {
        public string MessageCreate(Order order, Car car)
        {
            return $"Dear Mr/Mrs {order.Firstname} {order.Lastname}," +
                $"You made an order number {order.Id} to rent a {car.Manufacturer} {car.Model} for dates between {order.Startdate.Date} and {order.Enddate.Date}." +
                $"You could take your car between 8:00 and 22:00 {order.Startdate.Date} and return between 8:00 and 22:00 {order.Enddate.Date}" +
                $"You can take the car at the parking near the office. Our office is locate Lviv, Horodotska street, 280." +
                $"This rent will cost {order.Price} UAH. You have to pay rent bill when you will take the car." +
                $"Yours faithfully," +
                $"Car Rent Service";
        }

        public string MessageEdit(Order order, Car car)
        {
            return $"Dear Mr/Mrs {order.Firstname} {order.Lastname}," +
                $"You edited an order number {order.Id} to rent a {car.Manufacturer} {car.Model} for dates between {order.Startdate.Date} and {order.Enddate.Date}." +
                $"You could take your car between 8:00 and 22:00 {order.Startdate.Date} and return between 8:00 and 22:00 {order.Enddate.Date}" +
                $"You can take the car at the parking near the office. Our office is locate Lviv, Horodotska street, 280." +
                $"This rent will cost {order.Price} UAH. You have to pay rent bill when you will take the car." +
                $"Yours faithfully," +
                $"Car Rent Service";
        }

        public string MessageDelete(Order order)
        {
            return $"Dear Mr/Mrs {order.Firstname} {order.Lastname}," +
                $"Your order number {order.Id} was successfully deleted." +
                $"We hope You were satisfied with our service." +
                $"Yours faithfully," +
                $"Car Rent Service";
        }
    }
}
