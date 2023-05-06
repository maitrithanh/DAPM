using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LTWNC.Models;


namespace CNPMNC.Controllers
{
    public class UserController : Controller
    {
        // GET: Register
        private LTWNCEntities database = new LTWNCEntities();


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
        public ActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var kh = database.KHACHHANGs.Find(id);
            if (kh == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDLKH = new SelectList(database.LOAIKHACHHANGs, "IDLKH", "TENLOAI", kh.IDLKH);
            return View(database.KHACHHANGs.Where(s => s.IDKH == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditUser([Bind(Include = "IDKH,MAKH,HOTENKH,EMAIL,SDT,DIACHI,AVATARKH,IDLKH")] KHACHHANG kh, HttpPostedFileBase UploadImage)
        {
            if (ModelState.IsValid)
            {
                var khachhang = database.KHACHHANGs.FirstOrDefault(khach => khach.IDKH == kh.IDKH);
                if (khachhang != null)
                {
                    khachhang.MAKH = kh.MAKH;
                    khachhang.HOTENKH = kh.HOTENKH;
                    khachhang.EMAIL = kh.EMAIL;
                    khachhang.SDT = kh.SDT;
                    khachhang.DIACHI = kh.DIACHI;
                    if (UploadImage != null)
                    {
                        string filename = Path.GetFileNameWithoutExtension(UploadImage.FileName);
                        string extent = Path.GetExtension(UploadImage.FileName);
                        filename = filename + extent;
                        khachhang.AVATARKH = "~/Content/Images/" + filename;
                        UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), filename));
                    }
                    khachhang.IDLKH = kh.IDLKH;
                }
                database.SaveChanges();
                return Redirect("/Management/ThanhVien");
            }
            ViewBag.IDLKH = new SelectList(database.LOAIKHACHHANGs, "IDLKH", "TENLOAI", kh.IDLKH);
            return View(kh);
        }

        public ActionResult DeleteUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var kh = database.KHACHHANGs.Find(id);
            if (kh == null)
            {
                return HttpNotFound();
            }
            return View(kh);
        }


        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                KHACHHANG kh = database.KHACHHANGs.Find(id);
                database.KHACHHANGs.Remove(kh);
                database.SaveChanges();
                return Redirect("/Management/ThanhVien");
            }
            catch
            {
                KHACHHANG kh = database.KHACHHANGs.Find(id);
                ViewBag.ErrorDelete = "Xoá không thành công! thông tin khách hàng đang được sử dụng";
                return View(kh);
            }
        }

        public ActionResult DetailUser(int id)
        {
            return View(database.KHACHHANGs.Where(kh => kh.IDKH == id).FirstOrDefault());
        }
    }
}