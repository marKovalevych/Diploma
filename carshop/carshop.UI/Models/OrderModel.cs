using System;

namespace carshop.carshop.UI.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Car_Id { get; set; }
        public string Email { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public double Price { get; }
    }
}
