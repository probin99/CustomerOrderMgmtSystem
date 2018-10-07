using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OrderMgmtUsingEF.Context;
using OrderMgmtUsingEF.Models;

namespace OrderMgmtUsingEF.Controllers
{
    public class CustomersController : Controller
    {
        private ModelContext db = new ModelContext();

        // GET: Customers/Index
        public ActionResult Index()
        {
            return View();
        }
        
        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,Password,CustomerName,CustomerAddress")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Create", "Orders", new { id =  customer.CustomerID});
            }
            return View(customer);
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
