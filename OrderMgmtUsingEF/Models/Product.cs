using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderMgmtUsingEF.Models
{
    public class Product
    {
        public Product()
        {
            OrderItems = new HashSet<OrderItem>();
        }
        [Key]
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public bool IsCheck { get; set; }

        public double UnitPrice { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
    public class ProductList:Order
    {
        public List<Product> Products { get; set; }
    }
}