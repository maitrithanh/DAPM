using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWNC.Models;

namespace LTWNC.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult ShoppingCart()
        {
            return View();
        }
        public ActionResult ThanhToan()
        {
            return View();
        }
    }
}