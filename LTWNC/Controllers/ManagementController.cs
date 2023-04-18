using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWNC.Models;


namespace CNPMNC.Controllers
{

    public class ManagementController : Controller
    {
        LTWNCEntities database = new LTWNCEntities();
        // GET: Management
        public ActionResult Index()
        {
            TAIKHOAN user = Session["User"] as TAIKHOAN;
            if (user == null || user.VAITRO != "ADMIN")
            {
                return RedirectToAction("Login", "Home");
            }
            var dsDonHang = database.DONHANGs.ToList();
            var count = database.DONHANGs.ToList().Count();
            Session["SoDonHang"] = count;
            var doanhthu = database.CTDHs.Sum(s => s.THANHTIEN);
            Session["DoanhThu"] = doanhthu;
            var khachhang = database.KHACHHANGs.ToList().Count();
            Session["SoKhachHang"] = khachhang;
            var account = database.TAIKHOANs.ToList().Count();
            Session["SoAccount"] = account;
            return View(dsDonHang);
        }

        public ActionResult ProductsManagement()
        {
            TAIKHOAN user = Session["User"] as TAIKHOAN;
            if (user == null || user.VAITRO != "ADMIN")
            {
                return RedirectToAction("Login", "User");
            }
            var listProductsManagement = database.SANPHAMs.OrderByDescending(sp => sp.TENSP).ToList();
            return View(listProductsManagement);
        }

        public ActionResult QuanLyDonHang()
        {
            return View(database.DONHANGs.ToList());
        }

        public ActionResult ThanhVien()
        {
            TAIKHOAN user = Session["User"] as TAIKHOAN;
            if (user == null || user.VAITRO != "ADMIN")
            {
                return RedirectToAction("Login", "User");
            }
            var thanhvien = database.KHACHHANGs.OrderByDescending(tv => tv.IDKH).ToList();
            return View(thanhvien);
        }

        [HttpGet]
        public ActionResult Tracking(int? id)
        {
            var trackingOrder = database.CTGIAOHANGs.Where(s => s.IDDH == id).ToList();
            return View(trackingOrder);
        }
        [HttpPost]
        public ActionResult Tracking(int id, FormCollection form)
        {
            CTGIAOHANG gh = new CTGIAOHANG();
            if (string.IsNullOrEmpty(gh.VITRIGIAO))
                ModelState.AddModelError(string.Empty, "Vị trí giao không được để trống");
            try
            {
                gh.IDDH = id;
                gh.VITRIGIAO = form["VITRIGIAO"];
                gh.NGAYCAPNHAT = DateTime.Now;
                database.CTGIAOHANGs.Add(gh);
                database.SaveChanges();
                var trackingOrder = database.CTGIAOHANGs.Where(s => s.IDDH == id).ToList();
                return View(trackingOrder);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
    }
}