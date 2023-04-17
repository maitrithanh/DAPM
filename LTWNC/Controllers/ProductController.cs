using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWNC.Models;
using PagedList;
namespace LTWNC.Controllers
{
    public class ProductController : Controller
    {
        LTWNCEntities1 database = new LTWNCEntities1();
        // GET: Product
        public ActionResult Shop(int? page,string SearchString = "", double min = double.MinValue, double max= double.MaxValue )
        {
            int pageSize = 12;
            int pageNum = (page ?? 1);
            if (SearchString != "")
            {
                var listProducts = database.SANPHAMs.Where(s => s.TENSP.Contains(SearchString)).ToList();
                return View(listProducts.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var listProducts = database.SANPHAMs.Where(sp =>(double)sp.DONGIA >= min && (double)sp.DONGIA <= max).ToList();
                return View(listProducts.ToPagedList(pageNum, pageSize));
            }

        }
        [HttpGet]
        public ActionResult LayDanhMuc()
        {
            return PartialView(database.DANHMUCs.ToList());
        }
        public ActionResult SPTheoDanhMuc(string id, int? page, string SearchString = "", double min = double.MinValue, double max = double.MaxValue)
        {
            int pageSize = 12;
            int pageNum = (page ?? 1);
            if (SearchString != "")
            {
                var listProducts = database.SANPHAMs.Where(s => s.TENSP.Contains(SearchString)).ToList();
                return View(listProducts.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var listProducts = database.SANPHAMs.Where(sp => sp.DANHMUC.IDDANHMUC == id && (double)sp.DONGIA >= min && (double)sp.DONGIA <= max).ToList();
                return View(listProducts.ToPagedList(pageNum, pageSize));
            }
           
        }
        public ActionResult DemSPInDM(string id)
        {
            var listsp = database.SANPHAMs.Where(s => s.IDDANHMUC == id).ToList();
            return PartialView(listsp);
        }
        public ActionResult DemSPInGia(int? max, int? min)
        {
            var listsp = database.SANPHAMs.Where(sp => (double)sp.DONGIA >= min && (double)sp.DONGIA <= max).ToList();
            return PartialView(listsp);
        }
        [HttpGet]
        public ActionResult Feadback()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult Feadback(FEEDBACK fEEDBACK)
        {
            if (ModelState.IsValid)
            {
                var check = database.FEEDBACKs.Where(s => s.IDFEEDBACK == fEEDBACK.IDFEEDBACK).FirstOrDefault();
                if (check == null)
                {
                    int idkh, idsp;
                    int.TryParse(Convert.ToString(Session["IDKH"]), out idkh);
                    int.TryParse(Convert.ToString(Session["IDSP"]), out idsp);
                    fEEDBACK.IDKH = idkh;
                    fEEDBACK.IDSP = idsp;
                    database.FEEDBACKs.Add(fEEDBACK);
                    database.SaveChanges();
                    return RedirectToAction("PurchaseOrder","Profile");
                }
                else
                {

                    return Content("Bình luận không thành công");
                }
            }
            return PartialView();
        }
        public ActionResult InfoSPFeeback(int id)
        {
            var sp = database.SANPHAMs.Where(s => s.IDSP == id).FirstOrDefault();
            return PartialView(sp);
        }
    }
}