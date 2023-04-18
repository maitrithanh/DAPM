using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWNC.Models;

namespace CNPMNC.Controllers
{
    public class NotiController : Controller
    {
        // GET: Noti
        LTWNCEntities database = new LTWNCEntities();
        public ActionResult NotificationPartial()
        {
            var dsDonHang = database.DONHANGs.Where(dh => dh.TRANGTHAIDH == 1).ToList();
            Session["DonHangMoi"] = dsDonHang;
            return PartialView(dsDonHang);
        }
    }
}