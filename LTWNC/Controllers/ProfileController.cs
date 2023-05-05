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
            var sp = database.CTDHs.Where(s => s.IDDH == id).FirstOrDefault();
            return PartialView(sp);
        }
        public ActionResult datatable(int id)
        {
            var sp = database.CTDHs.Where(s => s.IDDH == id).FirstOrDefault();
            Session["IDSP"] = sp.IDSP;
            return PartialView(sp);
        }
        public ActionResult Layanh(int id)
        {
            var kt = database.KHACHHANGs.Where(s => s.IDKH == id).FirstOrDefault();
            return PartialView(kt);
        }
        [HttpGet]
        public ActionResult TrangThaiCB(int id)
        {
            var sp = database.CTDHs.Where(s => s.IDDH == id).FirstOrDefault();
            Session["IDSP"] = sp.IDSP;
            return PartialView(sp);
        }
        public ActionResult CancelOrder(int id)
        {
            return PartialView(database.DONHANGs.Where(s => s.IDDH == id).FirstOrDefault());
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
    }
}