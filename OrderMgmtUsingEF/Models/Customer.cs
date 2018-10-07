using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrderMgmtUsingEF.Models
{
    public class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public int CustomerID { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string CustomerAddress { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

    }
}