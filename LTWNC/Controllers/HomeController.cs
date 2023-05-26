using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using LTWNC.Models;

namespace LTWNC.Controllers
{
    public class HomeController : Controller
    {
        LTWNCEntities database = new LTWNCEntities();
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
        public ActionResult KhaoSat()
        {
            var ks = database.KHAOSATs.OrderByDescending(x => x.IDKHAOSAT).ToList();
            ViewBag.KhaoSat = ks.First();
            return View();
        }
        [HttpPost]
        public ActionResult KhaoSat(KHAOSAT ct)
        {
            var ks = database.KHAOSATs.Find(ct.IDKHAOSAT);
            ks.TRALOI = ct.TRALOI;
            database.SaveChanges();
            var ksat = database.KHAOSATs.OrderByDescending(x => x.IDKHAOSAT).ToList();
            ViewBag.KhaoSat = ksat.First();
            ViewBag.Msg = "OK";
            return View();
        }

        public ActionResult DanhMuc()
        {
            return PartialView(database.DANHMUCs.ToList());
        }

        public ActionResult DanhMucFooter()
        {
            return PartialView(database.DANHMUCs.ToList());
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(TAIKHOAN user, KHACHHANG kh)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(user.USERNAME))
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập không được để trống");
                if (string.IsNullOrEmpty(user.MATKHAU))
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");
                if (string.IsNullOrEmpty(kh.EMAIL))
                    ModelState.AddModelError(string.Empty, "Email không được để trống");
                if (string.IsNullOrEmpty(kh.HOTENKH))
                    ModelState.AddModelError(string.Empty, "Họ tên không được để trống");
                if (string.IsNullOrEmpty(kh.SDT))
                    ModelState.AddModelError(string.Empty, "Số điện thoại không được để trống");
                //Kiểm tra xem có người nào đã đăng kí với tên đăng nhập này hay chưa
                var khachhang = database.TAIKHOANs.FirstOrDefault(k => k.USERNAME == user.USERNAME);
                if (khachhang != null)
                    ModelState.AddModelError(string.Empty, "Đã có người đăng kí tên này");
                if (ModelState.IsValid)
                {
                    database.TAIKHOANs.Add(user);
                    database.SaveChanges();
                    kh.MAKH = Convert.ToString(user.ID);
                    database.KHACHHANGs.Add(kh);
                    database.SaveChanges();
                }
                else
                {
                    return View();
                }
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(TAIKHOAN user)
        {

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(user.USERNAME))
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập không được để trống");
                if (string.IsNullOrEmpty(user.MATKHAU))
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");
                if (ModelState.IsValid)
                {
                    var taikhoan = database.TAIKHOANs.FirstOrDefault(k => k.USERNAME == user.USERNAME && k.MATKHAU == user.MATKHAU);
                    if (taikhoan != null)
                    {
                        var IDUSER = Convert.ToString(taikhoan.ID);
                        var khachhang = database.KHACHHANGs.FirstOrDefault(kh => kh.MAKH == IDUSER);
                        var nhanvien = database.NHANVIENs.FirstOrDefault(kh => kh.IDNV == IDUSER);
                        var userLogin = database.TAIKHOANs.FirstOrDefault(kh => kh.ID == taikhoan.ID);
                        ViewBag.ThongBao = "Đăng nhập thành công";
                        Session["TaiKhoan"] = khachhang;
                        Session["User"] = userLogin;
                        Session["MaUser"] = taikhoan.ID;
                        Session["NhanVien"] = nhanvien;
                        if (taikhoan.VAITRO != "ADMIN")
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Management");
                        }
                    }

                    else
                        ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return View();
        }
        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}