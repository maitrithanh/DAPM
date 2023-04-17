using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWNC.Models;

namespace LTWNC.Controllers
{
    public class HomeController : Controller
    {
        LTWNCEntities1 database = new LTWNCEntities1();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(CONTACT ct)
        {
            database.CONTACTs.Add(ct);
            database.SaveChanges();
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(TAIKHOAN _tk)
        {
            if (ModelState.IsValid)
            {
                var check = database.TAIKHOANs.FirstOrDefault(s => s.ID == _tk.ID);
                if (check == null)
                {

                    database.Configuration.ValidateOnSaveEnabled = true;
                    database.TAIKHOANs.Add(_tk);
                    database.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Tài khoản email đã tồn tại ! Vui lòng sử dụng tài khoản email khác";
                    return View();
                }

            }
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string USERNAME, string MATKHAU)
        {
            if (ModelState.IsValid)
            {
                var data = database.TAIKHOANs.Where(s => s.USERNAME.Equals(USERNAME) && s.MATKHAU.Equals(MATKHAU)).ToList();
                if (data.Count() > 0)
                {
                    // add session :
                    Session["ID"] = data.FirstOrDefault().ID + " " + data.FirstOrDefault().ID;
                    Session["USERNAME"] = data.FirstOrDefault().USERNAME;
                    Session["MATKHAU"] = data.FirstOrDefault().MATKHAU;
                    Session["VAITRO"] = data.FirstOrDefault().VAITRO;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Login Failed";
                    return RedirectToAction("Register");
                }

            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}