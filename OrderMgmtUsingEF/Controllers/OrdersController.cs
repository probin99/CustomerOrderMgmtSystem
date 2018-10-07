using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using OrderMgmtUsingEF.Context;
using OrderMgmtUsingEF.Models;
using OrderMgmtUsingEF.ViewModel;

namespace OrderMgmtUsingEF.Controllers
{
    public class OrdersController : Controller
    {
        private ModelContext db = new ModelContext();

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Customer);
            return View(orders.ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create(int id)
        {
            List<Product> pr = new List<Product>
            {
                new Product() { ProductID = 1, ProductName = "Coffee", IsCheck = false, UnitPrice = 3.5 },
                new Product() { ProductID = 2, ProductName = "Pizza", IsCheck = false, UnitPrice = 20 },
                new Product() { ProductID = 3, ProductName = "Salad", IsCheck = false, UnitPrice = 9.75 },
                new Product() { ProductID = 4, ProductName = "Dumpling", IsCheck = false, UnitPrice = 13 },
                new Product() { ProductID = 5, ProductName = "Donut", IsCheck = false, UnitPrice = 2.75 }
            };

            ProductList prList = new ProductList
            {
                Products = pr
            };

            var queryProductListinDB = db.Products;
            if (queryProductListinDB.Count() == 0)
            {
                db.Products.AddRange(pr);
                db.SaveChanges();
            }
            
            prList.Customer = db.Customers.Find(id);
            return View(prList);
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductList pl)
        {
            //Total Price calculation and save data in Order table
            double totalPrice = 0;
            foreach(var item in pl.Products)
            {
                if(item.IsCheck)
                {
                    totalPrice += item.UnitPrice;
                }
            }

            var value = pl.Customer.CustomerID;

            Order order = new Order()
            {
                CustomerID = pl.Customer.CustomerID,
                OrderStatus = "Ordered",
                TotalPrice = totalPrice
            };

            //Save each item in OrderItem table
            List<OrderItem> orderItemList = new List<OrderItem>();
            foreach (var item in pl.Products)
            {
                if (item.IsCheck)
                {
                    OrderItem orderItem = new OrderItem()
                    {
                        OrderID = pl.OrderID,
                        ProductID = item.ProductID
                    };
                    orderItemList.Add(orderItem);
                }
            }
            db.Orders.Add(order); //Save Order entry
            db.OrderItems.AddRange(orderItemList); //save all products entry in one order 
            db.SaveChanges();

            return RedirectToAction("Create");
        }

        [HttpGet]
        //Get: OrderController/ConfirmedOrders
        public ActionResult ConfirmedOrders(int id)
        {
            string orderStatus = "Ordered";
            var allConfirmedOrders = db.Database.SqlQuery<CustomerOrderProduct>
            ("SELECT Orders.OrderID, Customers.CustomerID, Customers.CustomerName, Products.ProductID, Products.ProductName, Products.UnitPrice, Orders.TotalPrice, Orders.OrderStatus FROM(((Orders INNER JOIN Customers ON Orders.CustomerID = Customers.CustomerID) INNER JOIN OrderItems ON Orders.OrderID = OrderItems.OrderID) INNER JOIN Products ON Products.ProductID = OrderItems.ProductID) Where Customers.CustomerID = " + id + "").ToList();
            CustomerOrderProduct obj = new CustomerOrderProduct()
            {
                OrderList = allConfirmedOrders.Where(a=>a.OrderStatus == orderStatus).ToList()
            };

            //Find all orders
            List<int> orderIDList = new List<int>();
            foreach (var a in obj.OrderList)
            {
                if (!orderIDList.Contains(a.OrderID))
                {
                    orderIDList.Add(a.OrderID);
                }
            }
            ViewBag.orderIDList = orderIDList;

            //Find all product name and concatinate
            string productString = string.Empty;
            List<string> productNameList = new List<string>();

            foreach (var item in orderIDList)
            {
                var stringsColl = (obj.OrderList.Where(a => a.OrderID == item).Select(a => a.ProductName));
                foreach(var items in stringsColl)
                {
                    productString = productString + ", " + items;
                }
                string commaRemoved = productString.Substring(1, productString.Length-1);
                productNameList.Add(commaRemoved);
                commaRemoved = string.Empty;
                productString = string.Empty;
            }
            ViewBag.stringCollectionProductName = productNameList;
            
            Session["CustomerID"] = id;
            return View(obj.OrderList);
        }

        public ActionResult CustomerCancelOrder(int id)
        {
            int CustomerID = (int)Session["CustomerID"];
            int OrderID = id;
            Session["OrderID"] = id;

            var toCancelOrder = db.Database.SqlQuery<CustomerOrderProduct>
            ("SELECT Orders.OrderID, Customers.CustomerID, Customers.CustomerName, Products.ProductID, Products.ProductName, Products.UnitPrice, Orders.TotalPrice, Orders.OrderStatus FROM(((Orders INNER JOIN Customers ON Orders.CustomerID = Customers.CustomerID) INNER JOIN OrderItems ON Orders.OrderID = OrderItems.OrderID) INNER JOIN Products ON Products.ProductID = OrderItems.ProductID) Where Customers.CustomerID = "+ CustomerID +"").ToList();
            CustomerOrderProduct obj = new CustomerOrderProduct()
            {
                OrderList = toCancelOrder.Where(a=>a.OrderStatus == "Ordered" && a.OrderID == OrderID).ToList()
            };
            return View(obj.OrderList);
        }

        public ActionResult ConfirmCustomerCancel(int id)
        {
            foreach (var order in db.Orders.Where(a => a.OrderID == id))
            {
                order.OrderStatus = "Cancelled";
                db.Entry(order).State = EntityState.Modified;
            }
            db.SaveChanges();
            return RedirectToAction("ConfirmedOrders", new { id = (int)Session["CustomerID"]});
        }

        public ActionResult CancelledOrders(int id)
        {
            var allConfirmedOrders = db.Database.SqlQuery<CustomerOrderProduct>
            ("SELECT Orders.OrderID, Customers.CustomerID, Customers.CustomerName, Products.ProductID, Products.ProductName, Products.UnitPrice, Orders.TotalPrice, Orders.OrderStatus FROM(((Orders INNER JOIN Customers ON Orders.CustomerID = Customers.CustomerID) INNER JOIN OrderItems ON Orders.OrderID = OrderItems.OrderID) INNER JOIN Products ON Products.ProductID = OrderItems.ProductID) Where Customers.CustomerID = " + id + "").ToList();
            CustomerOrderProduct obj = new CustomerOrderProduct()
            {
                OrderList = allConfirmedOrders.Where(a=>a.OrderStatus == "Cancelled").ToList()
            };

            //Find all orders
            List<int> orderIDList = new List<int>();
            foreach (var a in obj.OrderList)
            {
                if (!orderIDList.Contains(a.OrderID))
                {
                    orderIDList.Add(a.OrderID);
                }
            }
            ViewBag.orderIDList = orderIDList;

            //Find all product name and concatinate
            string productString = string.Empty;
            List<string> productNameList = new List<string>();

            foreach (var item in orderIDList)
            {
                var stringsColl = (obj.OrderList.Where(a => a.OrderID == item).Select(a => a.ProductName));
                foreach (var items in stringsColl)
                {
                    productString = productString + ", " + items;
                }
                string commaRemoved = productString.Substring(1, productString.Length - 1);
                productNameList.Add(commaRemoved);
                commaRemoved = string.Empty;
                productString = string.Empty;
            }
            ViewBag.stringCollectionProductName = productNameList;



            Session["CustomerID"] = id;
            return View(obj.OrderList);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int id)
        {
            //Get data from database
            int CustomerID = (int)Session["CustomerID"];
            int OrderID = id;
            Session["OrderID"] = id;
            Session["CustomerID"] = CustomerID;

            var toCancelOrder = db.Database.SqlQuery<CustomerOrderProduct>
            ("SELECT Orders.OrderID, Customers.CustomerID, Customers.CustomerName, Products.ProductID, Products.ProductName, Products.UnitPrice, Orders.TotalPrice, Orders.OrderStatus FROM(((Orders INNER JOIN Customers ON Orders.CustomerID = Customers.CustomerID) INNER JOIN OrderItems ON Orders.OrderID = OrderItems.OrderID) INNER JOIN Products ON Products.ProductID = OrderItems.ProductID) Where Customers.CustomerID = " + CustomerID + "").ToList();
            CustomerOrderProduct obj = new CustomerOrderProduct()
            {
                OrderList = toCancelOrder.Where(a => a.OrderStatus == "Ordered" && a.OrderID == OrderID).ToList()
            };

            //static productlist
            List<Product> pr = new List<Product>
            {
                new Product() { ProductID = 1, ProductName = "Coffee", IsCheck = false, UnitPrice = 3.5 },
                new Product() { ProductID = 2, ProductName = "Pizza", IsCheck = false, UnitPrice = 20 },
                new Product() { ProductID = 3, ProductName = "Salad", IsCheck = false, UnitPrice = 9.75 },
                new Product() { ProductID = 4, ProductName = "Dumpling", IsCheck = false, UnitPrice = 13 },
                new Product() { ProductID = 5, ProductName = "Donut", IsCheck = false, UnitPrice = 2.75 }
            };
            var previousSelectedProducts = obj.OrderList.Select(a => a.ProductName).ToArray();
            //check item selected previously
            foreach (var item in pr)
            {
                if(previousSelectedProducts.Contains(item.ProductName))
                {
                    item.IsCheck = true;
                }
            }

            ProductList prList = new ProductList
            {
                Products = pr
            };
            prList.Customer = db.Customers.Find(CustomerID);
            return View(prList);
        }

        // POST: Orders/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductList pl)
        {
            try
            {
                    int OrderID = (int)Session["OrderID"];
                    //Total Price calculation and save data in Order table
                    double totalPrice = 0;
                    foreach (var item in pl.Products)
                    {
                        if (item.IsCheck)
                        {
                            totalPrice += item.UnitPrice;
                        }
                    }

                    //save new total price in table
                    foreach (var item in db.Orders.Where(a => a.OrderID == OrderID))
                    {
                        Order order = new Order()
                        {
                            TotalPrice = totalPrice
                        };
                        db.Entry(order).State = EntityState.Modified;
                    }
                    db.SaveChanges();

                    //Save each item in OrderItem table
                    //Current data
                    List<OrderItem> currentOrders = new List<OrderItem>();
                    foreach (var item in pl.Products)
                    {
                        if (item.IsCheck)
                        {
                            OrderItem orderItem = new OrderItem()
                            {
                                OrderID = OrderID,
                                ProductID = item.ProductID
                            };
                            currentOrders.Add(orderItem);
                        }
                    }
                    //Database Data
                    List<OrderItem> dbOrders = new List<OrderItem>(db.OrderItems.Where(a => a.OrderID == OrderID).ToList());

                    //Compare two list with productlist
                    List<OrderItem> insertItem = new List<OrderItem>();
                    foreach (var item in pl.Products)
                    {
                        if (item.IsCheck && !dbOrders.Select(a => a.ProductID).Contains(item.ProductID))
                        {
                            OrderItem insertOrderItem = new OrderItem()
                            {
                                OrderID = OrderID,
                                ProductID = item.ProductID
                            };
                            db.OrderItems.Add(insertOrderItem);
                        }
                        else if (!item.IsCheck && dbOrders.Select(a => a.ProductID).Contains(item.ProductID))
                        {
                            OrderItem deleteOrderItem = new OrderItem()
                            {
                                OrderID = OrderID,
                                ProductID = item.ProductID
                            };
                            OrderItem orderItem = db.OrderItems.Find(deleteOrderItem.ProductID);
                            db.OrderItems.Remove(orderItem);
                        }
                    }
                    db.SaveChanges();
                return RedirectToAction("ConfirmedOrders", new { id = pl.Customer.CustomerID });
            }
            catch(DbUpdateConcurrencyException ex)
            {
                ex.Entries.Single().Reload();
                db.SaveChanges();
                return RedirectToAction("ConfirmedOrders", new { id = pl.Customer.CustomerID });
            }
        }

        public ActionResult CreateInvoice(int id)
        {            
            var allConfirmedOrders = db.Database.SqlQuery<CustomerOrderProduct>
            ("SELECT Orders.OrderID, Customers.CustomerID, Customers.CustomerName, Products.ProductID, Products.ProductName, Products.UnitPrice, Orders.TotalPrice, Orders.OrderStatus FROM(((Orders INNER JOIN Customers ON Orders.CustomerID = Customers.CustomerID) INNER JOIN OrderItems ON Orders.OrderID = OrderItems.OrderID) INNER JOIN Products ON Products.ProductID = OrderItems.ProductID) Where Customers.CustomerID = " + id + "").ToList();
           
            List<CustomerOrderProduct> allRecords = new List<CustomerOrderProduct>();
            foreach (var item in allConfirmedOrders)
            {
                CustomerOrderProduct obj2 = new CustomerOrderProduct()
                {
                    CustomerID = item.CustomerID,
                    OrderID = item.OrderID,
                    CustomerName = item.CustomerName,
                    ProductID = item.ProductID,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    OrderStatus = item.OrderStatus,
                    OrderList = allConfirmedOrders.ToList()
                };
                if (obj2.OrderStatus == "Ordered")
                {
                    allRecords.Add(obj2);
                }
            }
            

            ViewBag.invoiceNumber = DateTime.Now.ToString("yyyyMMddHHmmss");
            return View(allRecords);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
