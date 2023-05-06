using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWNC.Models
{
    public class CartItem
    {
        LTWNCEntities database = new LTWNCEntities();

        public int IDSP { get; set; }
        public string TENSP { get; set; }
        public string HINHANHSP { get; set; }
        public decimal DONGIA { get; set; }
        public int SOLUONG { get; set; }
        //public decimal GIATRI { get; set; }

        //public int IDKH { get; set; }

        //Thanh Tien
        public decimal THANHTIEN()
        {
            return SOLUONG * DONGIA;
        }

        //public decimal TONGTIEN()
        //{
        //    return THANHTIEN() * (GIATRI / 100);
        //}

        public CartItem(int IDSP)
        {
            this.IDSP = IDSP;

            var sanphamDB = database.SANPHAMs.Single(sp => sp.IDSP == this.IDSP);

            this.TENSP = sanphamDB.TENSP;
            this.HINHANHSP = sanphamDB.HINHSP;
            this.DONGIA = (decimal)sanphamDB.DONGIA;
            this.SOLUONG = 1;


            //var idkhachhang = database.KHACHHANGs.Single(kh => kh.IDKH == this.IDKH);
            //var Khuyenmaidb = database.KHUYENMAIs.Single(km => km.IDKM == this.IDKH);
            //this.GIATRI = (decimal)Khuyenmaidb.GIATRI;
        }
    }
}