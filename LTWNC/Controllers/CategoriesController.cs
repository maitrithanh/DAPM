using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWNC.Models;
using System.Net;


namespace CNPMNC.Controllers
{

    public class CategoriesController : Controller
    {
        LTWNCEntities database = new LTWNCEntities();

        // GET: Categories
        public ActionResult Index()
        {
            var categories = database.DANHMUCs.ToList();
            return View(categories);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(DANHMUC category)
        {
            try
            {
                database.DANHMUCs.Add(category);
                database.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return Content("LỖI TẠO MỚI CATEGORY");
            }
        }

        public ActionResult Details(int id)
        {
            var category = database.DANHMUCs.Where(c => c.ID == id).FirstOrDefault();
            return View(category);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var category = database.DANHMUCs.Where(c => c.ID == id).FirstOrDefault();
            return View(category);
        }
        [HttpPost]
        public ActionResult Edit(int id, DANHMUC category)
        {
            database.Entry(category).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new
                HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var category = database.DANHMUCs.Where(c => c.ID == id).FirstOrDefault();
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var category = database.DANHMUCs.Where(c => c.ID == id).FirstOrDefault();
                database.DANHMUCs.Remove(category);
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return Content("Không xóa được do có liên quan đến bảng khác");
            }
        }

        public ActionResult CategoryPartial()
        {
            var categoryList = database.DANHMUCs.ToList();
            return PartialView(categoryList);
        }

        public ActionResult CategoryGroup(string id)
        {
            var category = database.SANPHAMs.Where(sach => sach.IDDANHMUC == id).ToList();
            return PartialView("index", category);
        }
    }
}