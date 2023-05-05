using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LTWNC.Models;
using PagedList;
using PagedList.Mvc;
namespace LTWNC.Controllers
{
    public class ProductController : Controller
    {
        LTWNCEntities database = new LTWNCEntities();
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

        public ActionResult Create()
        {
            ViewBag.IDDANHMUC = new SelectList(database.DANHMUCs, "IDDANHMUC", "TENDANHMUC");
            ViewBag.IDNV = new SelectList(database.NHANVIENs, "IDNV", "HOTENNV");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDSP,TENSP,SOLUONG,DONGIA,MOTA,HINHSP,IDDANHMUC,IDNV")] SANPHAM sp, HttpPostedFileBase UploadImage)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(sp.TENSP))
                    ModelState.AddModelError(string.Empty, "Tên sản phẩm không được để trống");
                if (string.IsNullOrEmpty(Convert.ToString(sp.SOLUONG)))
                    ModelState.AddModelError(string.Empty, "Số lượng không được để trống");
                if (string.IsNullOrEmpty(Convert.ToString(sp.DONGIA)))
                    ModelState.AddModelError(string.Empty, "Đơn giá không được để trống");
                if (string.IsNullOrEmpty(sp.MOTA))
                    ModelState.AddModelError(string.Empty, "Mô tả không được để trống");

                if (string.IsNullOrEmpty(sp.IDDANHMUC))
                    ModelState.AddModelError(string.Empty, "Danh mục không được để trống");
                if (ModelState.IsValid)
                {
                    if (UploadImage != null)
                    {
                        string filename = Path.GetFileNameWithoutExtension(UploadImage.FileName);
                        string extent = Path.GetExtension(UploadImage.FileName);
                        filename = filename + extent;
                        sp.HINHSP = "~/Content/Images/" + filename;
                        UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), filename));
                    }
                    database.SANPHAMs.Add(sp);
                    database.SaveChanges();
                    return Redirect("/Management/ProductsManagement");
                }
            }
            ViewBag.IDDANHMUC = new SelectList(database.DANHMUCs, "IDDANHMUC", "TENDANHMUC", sp.IDDANHMUC);
            ViewBag.IDNV = new SelectList(database.NHANVIENs, "IDNV", "HOTENNV", sp.IDNV);
            return View();
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sp = database.SANPHAMs.Find(id);
            if (sp == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDDANHMUC = new SelectList(database.DANHMUCs, "IDDANHMUC", "TENDANHMUC", sp.IDDANHMUC);
            ViewBag.IDNV = new SelectList(database.NHANVIENs, "IDNV", "HOTENNV", sp.IDNV);
            return View(sp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDSP,TENSP,SOLUONG,DONGIA,MOTA,HINHSP,IDDANHMUC,IDNV")] SANPHAM sp, HttpPostedFileBase UploadImage)
        {
            if (ModelState.IsValid)
            {
                var sanPhamDB = database.SANPHAMs.FirstOrDefault(kh => kh.IDSP == sp.IDSP);
                if (sanPhamDB != null)
                {
                    sanPhamDB.TENSP = sp.TENSP;
                    sanPhamDB.SOLUONG = sp.SOLUONG;
                    sanPhamDB.DONGIA = sp.DONGIA;
                    sanPhamDB.MOTA = sp.MOTA;
                    if (UploadImage != null)
                    {
                        string filename = Path.GetFileNameWithoutExtension(UploadImage.FileName);
                        string extent = Path.GetExtension(UploadImage.FileName);
                        filename = filename + extent;
                        sanPhamDB.HINHSP = "~/Content/Images/" + filename;
                        UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), filename));
                    }
                    sanPhamDB.IDDANHMUC = sp.IDDANHMUC;
                    sanPhamDB.IDNV = sp.IDNV;
                }
                database.SaveChanges();
                return Redirect("/Management/ProductsManagement");
            }
            ViewBag.IDDANHMUC = new SelectList(database.DANHMUCs, "IDDANHMUC", "TENDANHMUC", sp.IDDANHMUC);
            ViewBag.IDNV = new SelectList(database.NHANVIENs, "IDNV", "HOTENNV", sp.IDNV);
            return View(sp);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sp = database.SANPHAMs.Find(id);
            if (sp == null)
            {
                return HttpNotFound();
            }
            return View(sp);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SANPHAM sp = database.SANPHAMs.Find(id);
            database.SANPHAMs.Remove(sp);
            database.SaveChanges();
            return Redirect("/Management/ProductsManagement");
        }

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
        public ActionResult LayHinh()
        {
            var hinh = database.SANPHAMs.ToList();
            return PartialView(hinh);
        }
        public ActionResult ProductDetail(int id = 2)
        {
            return View(database.SANPHAMs.Where(s => s.IDSP == id).FirstOrDefault());
        }
        private List<SANPHAM> ListSP(string id, int soluong)
        {
            return database.SANPHAMs.Where(s => s.IDDANHMUC == id).Take(soluong).ToList();
        }
        public ActionResult SanPhamCungLoai(string id)
        {
            var dsSanpham = ListSP(id, 3);
            return PartialView(dsSanpham);
        }
        public ActionResult SanPhamCoTheThich()
        {
            var listProducts = database.SANPHAMs.OrderByDescending(sp => sp.TENSP).ToList();
            return PartialView(listProducts);
        }
       public ActionResult Search(string SearchString = "")
        {
            var list = database.SANPHAMs.Where(s => s.TENSP.Contains(SearchString)).ToList();
            return RedirectToAction("Shop", list);
        }
    }
}