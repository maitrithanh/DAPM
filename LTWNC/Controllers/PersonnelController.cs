using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LTWNC.Models;
using System.Net;

namespace LTWNC.Controllers
{
    public class PersonnelController : Controller
    {
        // GET: Personnel
        LTWNCEntities database = new LTWNCEntities();
        // GET: Personnel
        public ActionResult Index()
        {
            var personnel = database.NHANVIENs.ToList();
            return View(personnel);
        }

        public ActionResult Create()
        {
            ViewBag.IDCV = new SelectList(database.CHUCVUs, "IDCV", "TENCV");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,IDNV,HOTENNV,EMAIL,DIACHI,SDT,AVATANV,IDCV")] NHANVIEN NV, HttpPostedFileBase UploadImage)
        {
            if (ModelState.IsValid)
            {

                if (string.IsNullOrEmpty(Convert.ToString(NV.EMAIL)))
                    ModelState.AddModelError(string.Empty, "Email không được để trống");
                if (string.IsNullOrEmpty(Convert.ToString(NV.DIACHI)))
                    ModelState.AddModelError(string.Empty, "Địa chỉ không được để trống");
                if (string.IsNullOrEmpty(NV.SDT))
                    ModelState.AddModelError(string.Empty, "Số điện thoại không được để trống");
                if (ModelState.IsValid)
                {
                    if (UploadImage != null)
                    {
                        string filename = Path.GetFileNameWithoutExtension(UploadImage.FileName);
                        string extent = Path.GetExtension(UploadImage.FileName);
                        filename = filename + extent;
                        NV.AVATARNV = "~/Content/Images/" + filename;
                        UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), filename));
                    }
                    database.NHANVIENs.Add(NV);
                    database.SaveChanges();
                    return Redirect("/Personnel/Index");
                }
            }
            ViewBag.IDCV = new SelectList(database.CHUCVUs, "IDCV", "TENCV", NV.IDCV);
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var nv = database.NHANVIENs.Where(s => s.ID == id).FirstOrDefault();
            ViewBag.IDCV = new SelectList(database.CHUCVUs, "IDCV", "TENCV");
            return View(nv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,IDNV,HOTENNV,EMAIL,DIACHI,SDT,AVATANV,IDCV")] NHANVIEN NV, HttpPostedFileBase UploadImage)
        {
            if (ModelState.IsValid)
            {
                var nhanvien = database.NHANVIENs.FirstOrDefault(nv => nv.ID == NV.ID);
                if (nhanvien != null)
                {
                    nhanvien.IDNV = NV.IDNV;
                    nhanvien.HOTENNV = NV.HOTENNV;
                    nhanvien.EMAIL = NV.EMAIL;
                    nhanvien.DIACHI = NV.DIACHI;
                    nhanvien.SDT = NV.SDT;
                    nhanvien.IDCV = NV.IDCV;
                    if (UploadImage != null)
                    {
                        string filename = Path.GetFileNameWithoutExtension(UploadImage.FileName);
                        string extent = Path.GetExtension(UploadImage.FileName);
                        filename = filename + extent;
                        nhanvien.AVATARNV = "~/Content/Images/" + filename;
                        UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), filename));
                    }

                }
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDCV = new SelectList(database.CHUCVUs, "IDCV", "TENCV", NV.IDCV);
            return View(NV);
        }
        public ActionResult Details(int id)
        {

            var nv = database.NHANVIENs.Where(s => s.ID == id).FirstOrDefault();
            ViewBag.IDCV = new SelectList(database.CHUCVUs, "IDCV", "TENCV", nv.IDCV);
            return View(nv);
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            var nv = database.NHANVIENs.Where(c => c.ID == id).FirstOrDefault();
            ViewBag.IDCV = new SelectList(database.CHUCVUs, "IDCV", "TENCV", nv.IDCV);
            return View(nv);
        }
        [HttpPost]
        public ActionResult Delete(int id, NHANVIEN nv)
        {
            try
            {
                nv = database.NHANVIENs.Where(c => c.ID == id).FirstOrDefault();
                database.NHANVIENs.Remove(nv);
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return Content("Không xóa được do có liên quan đến bảng khác");
            }
        }
    }
}