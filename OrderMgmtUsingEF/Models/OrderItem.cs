using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderMgmtUsingEF.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}