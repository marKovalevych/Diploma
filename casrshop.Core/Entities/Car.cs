using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace casrshop.Core
{
    public class Car
    {
        [Key]
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int Yearof { get; set; }
        public string Gearbox { get; set; }
        public double Price { get; set; }
        public double Insurance { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
