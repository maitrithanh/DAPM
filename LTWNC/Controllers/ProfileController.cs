using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWNC.Models;
namespace LTWNC.Controllers
{
    public class ProfileController : Controller
    {
        LTWNCEntities1 database = new LTWNCEntities1();
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
    }
}