using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWNC.Models;
namespace LTWNC.Controllers
{
    public class ProfileController : Controller
    {
        LTWNCEntities database = new LTWNCEntities();
        // GET: Profile
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PurchaseOrder()
        {
            int id = 1;
            Session["IDKH"] = 1;
            //int.TryParse(Convert.ToString(Session["IDKH"]), out id);
            return View(database.DONHANGs.Where(s => s.IDKH == id).ToList());
        }
        public ActionResult ListProductstatus(int id)
        {
            var dem = database.CTDHs.Where(s => s.IDDH == id).ToList();
            if (dem.Count < 1)
            {
                var sp = database.CTDHs.Where(s => s.IDDH == id).FirstOrDefault();
                return PartialView(sp);
            }
            else
            {
                var dh = database.CTDHs.Where(s => s.IDDH == id).ToList();
                return PartialView(dh);
            }

        }
        public ActionResult datatable(int id)
        {
            var dem = database.CTDHs.Where(s => s.IDDH == id).ToList();
            if (dem.Count < 1)
            {
                var sp = database.CTDHs.Where(s => s.IDDH == id).FirstOrDefault();
                return PartialView(sp);
            }
            else
            {
                var dh = database.CTDHs.Where(s => s.IDDH == id).ToList();
                return PartialView(dh);
            }
        }
        public ActionResult Layanh(int id)
        {
            var kt = database.KHACHHANGs.Where(s => s.IDKH == id).FirstOrDefault();
            return PartialView(kt);
        }
        [HttpGet]
        public ActionResult TrangThaiCB(int id)
        {
            var dem = database.CTDHs.Where(s => s.IDDH == id).ToList();
            if (dem.Count < 1)
            {
                var sp = database.CTDHs.Where(s => s.IDDH == id).FirstOrDefault();

                return PartialView(sp);
            }
            else
            {
                var dh = database.CTDHs.Where(s => s.IDDH == id).ToList();
                return PartialView(dh);
            }
        }
        public ActionResult DetailSPHuy(int id)
        {
            var sp = database.CTDHs.Where(s => s.IDDH == id).ToList();
            return PartialView(sp);
        }
        public ActionResult CancelOrder(int id)
        {
            return View(database.DONHANGs.Where(s => s.IDDH == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult CancelOrder(int id, DONHANG dh)
        {
            try
            {
                database.Configuration.ValidateOnSaveEnabled = false;
                database.Entry(dh).State = System.Data.Entity.EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("PurchaseOrder");
            }
            catch (Exception e)
            {
                return Content(Convert.ToString(e));
            }
        }
        public ActionResult TrackingChitiet(int? id)
        {
            var trackingOrder = database.CTGIAOHANGs.Where(s => s.IDDH == id).ToList();
            return PartialView(trackingOrder);
        }
        public ActionResult ChitietDonHang(int id, int idsp)
        {
            var donhang = database.DONHANGs.Where(s => s.IDDH == id).FirstOrDefault();
            var sp = database.CTDHs.Where(s => s.IDDH == donhang.IDDH && s.IDSP == idsp).FirstOrDefault();
            Session["IDSP"] = sp.IDSP;
            return View(donhang);
        }
        public ActionResult HienDHFeedback(int id, int idsp)
        {
            var sp = database.CTDHs.Where(s => s.IDDH == id && s.IDSP == idsp).FirstOrDefault();
            Session["IDSP"] = sp.IDSP;
            return PartialView(sp);
        }
        public ActionResult LaySPFB(int id, int idsp)
        {
            var sp = database.CTDHs.Where(s => s.IDDH == id && s.IDSP == idsp).FirstOrDefault();
            return PartialView(sp);
        }

        [HttpGet]
        public ActionResult Edit()
        {
            string id = Convert.ToString(Session["MaUser"]);
            var taikhoan = database.KHACHHANGs.Where(s => s.MAKH == id).FirstOrDefault();
            Session["IDKH"] = taikhoan.IDKH;
            return View(taikhoan);
        }
        [HttpPost]
        public ActionResult Edit(string id, KHACHHANG kh, HttpPostedFileBase UploadImage)
        {
            try
            {
                database.Configuration.ValidateOnSaveEnabled = false;
                database.Entry(kh).State = System.Data.Entity.EntityState.Modified;
                if (UploadImage != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(UploadImage.FileName);
                    string extent = Path.GetExtension(UploadImage.FileName);
                    filename = filename + extent;
                    kh.AVATARKH = "~/Content/Images/" + filename;
                    UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), filename));
                }
                database.SaveChanges();
                return RedirectToAction("Edit");
            }
            catch (Exception e)
            {
                return Content(Convert.ToString(e));
            }
        }
        public ActionResult Tracking(int? id)
        {
            var trackingOrder = database.CTGIAOHANGs.Where(s => s.IDDH == id).ToList();
            return View(trackingOrder);
        }

        [HttpGet]
        public ActionResult XacNhanDonHang(int? id)
        {
            return PartialView(database.DONHANGs.Where(s => s.IDDH == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult XacNhanDonHang(DONHANG dh)
        {

            database.Configuration.ValidateOnSaveEnabled = false;
            database.Entry(dh).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            return RedirectToAction("QuanLyDonHang", "Management");
        }

        public ActionResult DuyetDon(int id)
        {
            var sp = database.CTDHs.Where(s => s.IDDH == id).FirstOrDefault();
            Session["IDSP"] = sp.IDSP;
            return PartialView(sp);
            //return PartialView(database.DONHANGs.Where(s => s.IDDH == id).FirstOrDefault());
        }
    }
}