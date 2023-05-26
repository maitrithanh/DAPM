using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
                return RedirectToAction("Login", "Home");
            }
            var listProductsManagement = database.SANPHAMs.OrderByDescending(sp => sp.TENSP).ToList();
            return View(listProductsManagement);
        }

        //public ActionResult QuanLyDonHang()
        //{
        //    return View(database.DONHANGs.ToList());
        //}

        public ActionResult ThanhVien()
        {
            TAIKHOAN user = Session["User"] as TAIKHOAN;
            if (user == null || user.VAITRO != "ADMIN")
            {
                return RedirectToAction("Login", "Home");
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


        public ActionResult KhaoSat()
        {
            TAIKHOAN user = Session["User"] as TAIKHOAN;
            if (user == null || user.VAITRO != "ADMIN")
            {
                return RedirectToAction("Login", "Home");
            }
            var listTs = database.KHAOSATs.OrderByDescending(sp => sp.IDKHAOSAT).ToList();
            return View(listTs);
        }

        public ActionResult CreateKS()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TENKHAOSAT,NOIDUNG")] KHAOSAT sp)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(sp.TENKHAOSAT))
                    ModelState.AddModelError(string.Empty, "Tên khảo sát không được để trống");
                if (string.IsNullOrEmpty(sp.NOIDUNG))
                    ModelState.AddModelError(string.Empty, "Nội dung không được để trống");

                if (ModelState.IsValid)
                {

                    database.KHAOSATs.Add(sp);
                    database.SaveChanges();
                    return Redirect("/Management/KhaoSat");
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditKS(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sp = database.KHAOSATs.Find(id);
            if (sp == null)
            {
                return HttpNotFound();
            }
            return View(sp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDKHAOSAT,TENKHAOSAT,NOIDUNG")] KHAOSAT sp)
        {
            if (ModelState.IsValid)
            {
                var sanPhamDB = database.KHAOSATs.FirstOrDefault(kh => kh.IDKHAOSAT == sp.IDKHAOSAT);
                if (sanPhamDB != null)
                {
                    sanPhamDB.TENKHAOSAT = sp.TENKHAOSAT;
                    sanPhamDB.NOIDUNG = sp.NOIDUNG;
                }
                database.SaveChanges();
                return Redirect("/Management/KhaoSat");
            }
            return View(sp);
        }

        public ActionResult DeleteKS(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sp = database.KHAOSATs.Find(id);
            if (sp == null)
            {
                return HttpNotFound();
            }
            return View(sp);
        }


        [HttpPost, ActionName("DeleteKS")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KHAOSAT sp = database.KHAOSATs.Find(id);
            database.KHAOSATs.Remove(sp);
            database.SaveChanges();
            return Redirect("/Management/KhaoSat");
        }

        public ActionResult DetailsKS(int id)
        {

            var nv = database.KHAOSATs.Where(s => s.IDKHAOSAT == id).FirstOrDefault();
            return View(nv);
        }

        //Quản lý đơn hàng (admin)
        public ActionResult QuanLyDonHang()
        {
            TAIKHOAN user = Session["User"] as TAIKHOAN;
            if (user == null || user.VAITRO != "ADMIN")
            {
                return RedirectToAction("Login", "User");
            }
            var listProductsManagement = database.DONHANGs.OrderByDescending(sp => sp.IDDH).ToList();
            return View(listProductsManagement);

        }
        public ActionResult DetailsDonHang(int? id)
        {

            DONHANG dh = database.DONHANGs.Find(id);

            var listOrder = from g in database.SANPHAMs
                            join p in database.CTDHs on g.IDSP equals p.IDSP
                            where p.IDDH == id
                            select new SANPHAM { IDSP = g.IDSP, TENSP = g.TENSP, DONGIA = g.DONGIA, HINHSP = g.HINHSP, SOLUONG = p.SOLUONG };
            ViewBag.order_item = listOrder;


            dh.ViewStatus = true;

            var count_order = (from or in database.DONHANGs where or.ViewStatus == false select or.IDDH).Count();

            if (count_order > 0)
            {
                Session["countnewcart"] = count_order;
            }

            database.Entry(dh).State = EntityState.Modified;
            database.SaveChanges();
            return View(dh);
        }
        public ActionResult XacThucDonHang(int id)
        {
            DONHANG dh = database.DONHANGs.Find(id);
            dh.TRANGTHAIGH = "1";
            database.Entry(dh).State = EntityState.Modified;
            database.SaveChanges();


            return RedirectToAction("DetailsDonHang", new { id = dh.IDDH });
        }
        public ActionResult HuyDonhang(int id)
        {
            DONHANG dh = database.DONHANGs.Find(id);
            dh.TRANGTHAIGH = "4";
            database.Entry(dh).State = EntityState.Modified;
            database.SaveChanges();


            return RedirectToAction("DetailsDonHang", new { id = dh.IDDH });
        }
        public ActionResult DeleteDH(int id)
        {
            DONHANG dh = database.DONHANGs.Find(id);

            if (dh.TRANGTHAIGH == "1" || dh.TRANGTHAIGH == "4")
            {
                var deleteCTGH = from CTGIAOHANG in database.CTGIAOHANGs where CTGIAOHANG.IDDH == id select CTGIAOHANG;
                var deleteDH = from DONHANG in database.DONHANGs where DONHANG.IDDH == id select DONHANG;
                var deleteCTDH = from CTDH in database.CTDHs where CTDH.IDDH == id select CTDH;
                foreach (var item in deleteCTGH)
                {

                    database.CTGIAOHANGs.Remove(item);

                }
                foreach (var item in deleteDH)
                {

                    database.DONHANGs.Remove(item);

                }
                foreach (var item in deleteCTDH)
                {

                    database.CTDHs.Remove(item);

                }
                database.DONHANGs.Remove(dh);
                database.SaveChanges();
                return RedirectToAction("QuanLyDonHang");
            }
            else
            {
                TempData["displayviss"] = "display:block!important";

                TempData["error_order_delete"] = "Để xóa đơn hàng bạn phải Xác Nhận đơn hàng hoặc đơn hàng phải trong trạng thái Đã Hủy";
                return RedirectToAction("DetailsDonHang", new { id = dh.IDDH });

            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                database.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult TongTienDonHang(int id)
        {

            var sp = database.CTDHs.Where(s => s.IDDH == id).ToList();
            //Session["IDSP"] = sp.IDSP;
            return PartialView(sp);
        public ActionResult test()
        {
            return View();
        }
    }
}