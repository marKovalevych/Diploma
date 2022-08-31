using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace casrshop.Core
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string Firstname{ get;  set; }
        public string Lastname { get; set; }
        public int Car_Id { get; set; }
        public string Email { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public double Price { get; set; }
        public virtual Car Car { get; set; }
    }
}
