using OrderMgmtUsingEF.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace OrderMgmtUsingEF.ViewModel
{
    public class CustomerOrderProduct
    {
        [Key]
        public int CustomerOrderProductID { get; set; }
        public int CustomerID { get; set; }
        public int OrderID { get; set; }
        public string  CustomerName { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
        public string OrderStatus { get; set; }
        public List<CustomerOrderProduct> OrderList { get; set; }
    }
}