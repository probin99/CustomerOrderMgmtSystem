using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OrderMgmtUsingEF.Models
{
    public class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }
        [Key]
        public int OrderID { get; set; }

        public int CustomerID { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public string OrderStatus { get; set; }

        public double TotalPrice { get; set; }
    }
}