using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWNC.Models;

namespace LTWNC.Controllers
{
    public class CartController : Controller
    {
        LTWNCEntities database = new LTWNCEntities();
        public List<CartItem> GetCart()
        {
            List<CartItem> myCart = Session["GioHang"] as List<CartItem>;
            //Nếu giỏ hàng chưa tồn tại thì tạo mới và đưa vào Session
            if (myCart == null)
            {
                myCart = new List<CartItem>();
                Session["GioHang"] = myCart;
            }
            return myCart;
        }

        public ActionResult AddToCart(int id/*, int idkm*/)
        {
            //Lấy giỏ hàng hiện tại
            List<CartItem> myCart = GetCart();
            CartItem currentProduct = myCart.FirstOrDefault(sp => sp.IDSP == id);
            // CartItem currentCoupon = myCart.FirstOrDefault(km => km.idkm == id);
            if (currentProduct == null)
            {
                currentProduct = new CartItem(id/*, idkm*/);
                myCart.Add(currentProduct);
            }
            /*else if (currentCoupon == null)
            {
                currentCoupon = new CartItem(idsp, idkm);
                myCart.Add(currentCoupon);
            }*/
            else
            {
                currentProduct.SOLUONG++;
                //Sản phẩm đã có trong giỏ thì tăng số lượng lên 1
            }

            return RedirectToAction("ShoppingCart", "Cart", new { id = id/*,idkm*/});
        }



        private int GetTotalNumber()
        {
            int totalNumber = 0;
            List<CartItem> myCart = GetCart();
            if (myCart != null)
                totalNumber = myCart.Sum(sp => sp.SOLUONG);
            return totalNumber;
        }

        private decimal GetTotalPrice()
        {
            decimal totalPrice = 0;
            List<CartItem> myCart = GetCart();
            if (myCart != null)
                totalPrice = myCart.Sum(sp => sp.THANHTIEN());
            return totalPrice;
        }

        private decimal GetDiscountPrice()
        {
            decimal totalPrice = 0;
            decimal tempprice = 0;
            List<CartItem> myCart = GetCart();
            if (myCart != null)
            {
                totalPrice = myCart.Sum(sp => sp.THANHTIEN());
                if (Session["TaiKhoan"] != null)
                {
                    tempprice = totalPrice * (@Convert.ToDecimal(Session["GIATRI"]) / 100);
                    totalPrice -= tempprice;
                }
            }
            return totalPrice;
        }

        private decimal GetTotalPriceAfterDisCount()
        {
            decimal totalPrice = 0;
            List<CartItem> myCart = GetCart();
            if (myCart != null)
            {
                totalPrice = myCart.Sum(sp => sp.THANHTIEN());
                if (Session["TaiKhoan"] != null)
                {
                    totalPrice *= (@Convert.ToDecimal(Session["GIATRI"]) / 100);
                }
            }
            return totalPrice;
        }

        public ActionResult CartPartial()
        {
            ViewBag.TotalNumber = GetTotalNumber();
            ViewBag.TotalPrice = GetTotalPrice();
            return PartialView();
        }

        public ActionResult DeleteCartItem(int id)
        {
            List<CartItem> myCart = GetCart();
            //Lấy sản phẩm trong giỏ hàng
            var currentProduct = myCart.FirstOrDefault(p => p.IDSP == id);
            if (currentProduct != null)
            {
                myCart.RemoveAll(p => p.IDSP == id);
                return RedirectToAction("ShoppingCart"); //Quay về trang giỏ hàng
            }
            return RedirectToAction("ShoppingCart"); //Quay về trang giỏ hàng
        }

        public ActionResult UpdateCartItem(int id, int Number)
        {
            List<CartItem> myCart = GetCart();
            //Lấy sản phẩm trong giỏ hàng
            var currentProduct = myCart.FirstOrDefault(p => p.IDSP == id);
            if (currentProduct != null)
            {
                //Cập nhật lại số lượng tương ứng 
                //Lưu ý số lượng phải lớn hơn hoặc bằng 1
                currentProduct.SOLUONG = Number;
            }
            return RedirectToAction("ShoppingCart"); //Quay về trang giỏ hàng
        }

        public ActionResult ConfirmCart(FormCollection form)
        {
            List<CartItem> myCart = GetCart();
            KHACHHANG khach = new KHACHHANG();

            if (Session["TaiKhoan"] == null)
            {
                khach.IDKH = 3;
                khach.DIACHI = form["DIACHI"];
                khach.HOTENKH = form["HOTENKH"];
                khach.SDT = form["SDT"];
                Session["TaiKhoan"] = khach;
            }

            Session["Pttt"] = form["PHUONGTHUCTHANHTOAN"];

            if (Session["TaiKhoan"] == null) //Chưa đăng nhập
                return RedirectToAction("Login", "User");
            if (myCart == null || myCart.Count == 0) //Chưa có giỏ hàng hoặc chưa có sp
            {
                ViewBag.ThongBaoCartNull = "Giỏ Hàng Trống Vui Lòng Thêm Sản Phẩm Vào Giỏ Hàng Trước Khi Đặt Hàng!";
                return View("ShoppingCart");
            }
            ViewBag.TotalNumber = GetTotalNumber();
            ViewBag.TotalPrice = GetTotalPrice();
            return View(myCart); //Trả về View xác nhận đặt hàng
        }

        public ActionResult AgreeCart(FormCollection form)
        {
            KHACHHANG khach = Session["TaiKhoan"] as KHACHHANG;
            //Khách
            List<CartItem> myCart = GetCart(); //Giỏ hàng
            DONHANG DonHang = new DONHANG(); //Tạo mới đơn đặt hàng DonHang.IDCus = khach.IDCus;
            DonHang.NGAYDAT = DateTime.Now;
            DonHang.IDKH = khach.IDKH;
            DonHang.TRANGTHAIDH = 1;
            DonHang.PHUONGTHUCTHANHTOAN = Convert.ToString(Session["Pttt"]);
            DonHang.DIACHINHANHANG = khach.DIACHI;
            DonHang.TRANGTHAIGH = "0";
            DonHang.TENNGUOINHAN = khach.HOTENKH;
            database.DONHANGs.Add(DonHang);
            database.SaveChanges();
            CTGIAOHANG GiaoHang = new CTGIAOHANG();
            GiaoHang.IDDH = DonHang.IDDH;
            GiaoHang.VITRIGIAO = "Đơn Hàng Chờ Vận Chuyển";
            GiaoHang.NGAYCAPNHAT = DateTime.Now;
            database.CTGIAOHANGs.Add(GiaoHang);
            database.SaveChanges();
            //Lần lượt thêm từng chi tiết cho đơn hàng trên
            foreach (var product in myCart)
            {
                CTDH chitiet = new CTDH();
                chitiet.IDDH = DonHang.IDDH;
                chitiet.IDSP = product.IDSP;
                chitiet.SOLUONG = product.SOLUONG;
                chitiet.DONGIA = product.DONGIA;
                database.CTDHs.Add(chitiet);
            }
            database.SaveChanges();
            //Xóa giỏ hàng
            Session["GioHang"] = null;
            return RedirectToAction("SuccessOrder", "Cart");
        }

        public ActionResult SuccessOrder()
        {
            return View();
        }

        public ActionResult CartNull()
        {
            return PartialView();
        }

        public ActionResult ResetAddress()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("ShoppingCart");
        }
        // GET: Cart
        public ActionResult ShoppingCart()
        {
            List<CartItem> myCart = GetCart();
            //Nếu giỏ hàng trống thì trả về trang ban đầu
            //if (myCart == null || myCart.Count == 0)
            //{
            //    return RedirectToAction("Index", "CustomerProducts");
            //}
            ViewBag.TotalNumber = GetTotalNumber();
            ViewBag.TotalPrice = GetTotalPrice();
            ViewBag.DiscountPrice = GetDiscountPrice();
            ViewBag.TotalPriceAfterDiscount = GetTotalPriceAfterDisCount();
            return View(myCart);
        }
        public ActionResult ThanhToan()
        {
            return View();
        }
    }
}