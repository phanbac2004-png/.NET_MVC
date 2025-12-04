using PhanXuanBac_22103100089.Data;
using PhanXuanBac_22103100089.ViewModels;
using Microsoft.AspNetCore.Mvc;
using PhanXuanBac_22103100089.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace PhanXuanBac_22103100089.Controllers
{
	public class CartController : Controller
	{
		private readonly Hshop2023Context db;

		public CartController(Hshop2023Context context)
		{
			db = context;
		}

		public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

		public IActionResult Index()
		{
			return View(Cart);
		}

		public IActionResult AddToCart(int id, int quantity = 1)
		{
			var gioHang = Cart;
			var item = gioHang.SingleOrDefault(p => p.MaHh == id);
			if (item == null)
			{
				var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
				if (hangHoa == null)
				{
					TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
					return Redirect("/404");
				}
				item = new CartItem
				{
					MaHh = hangHoa.MaHh,
					TenHH = hangHoa.TenHh,
					DonGia = hangHoa.DonGia ?? 0,
					Hinh = hangHoa.Hinh ?? string.Empty,
					SoLuong = quantity
				};
				gioHang.Add(item);
			}
			else
			{
				item.SoLuong += quantity;
			}

			HttpContext.Session.Set(MySetting.CART_KEY, gioHang);

			return RedirectToAction("Index");
		}

		public IActionResult RemoveCart(int id)
		{
			var gioHang = Cart;
			var item = gioHang.SingleOrDefault(p => p.MaHh == id);
			if (item != null)
			{
				gioHang.Remove(item);
				HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
			}
			return RedirectToAction("Index");
		}
		[Authorize]
		public IActionResult Checkout()
		{
			if(Cart.Count == 0)
            {
				return Redirect("/");
            }
            return View(Cart);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;
                var khachHang = new KhachHang();
                if (model.GiongKhachHang)
                {
                    khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                }

                var hoadon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.HoTen ?? khachHang.HoTen,
                    DiaChi = model.DiaChi ?? khachHang.DiaChi,
                    DienThoai = model.DienThoai ?? khachHang.DienThoai,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "GRAB",
                    MaTrangThai = 0,
                    GhiChu = model.GhiChu
                };

                db.Database.BeginTransaction();
                try
                {
                    db.Database.CommitTransaction();
                    db.Add(hoadon);
                    db.SaveChanges();

                    var gioHang = Cart;
                    var cthds = new List<ChiTietHd>();
                    foreach (var item in gioHang)
                    {
                        cthds.Add(new ChiTietHd
                        {
                            MaHd = hoadon.MaHd,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaHh = item.MaHh,
                            GiamGia = 0
                        });
                    }
                    db.AddRange(cthds);
                    db.SaveChanges();

                    var orderSummary = new OrderSuccessVM
                    {
                        OrderId = hoadon.MaHd,
                        CustomerName = hoadon.HoTen,
                        Address = hoadon.DiaChi,
                        Phone = hoadon.DienThoai,
                        OrderDate = hoadon.NgayDat,
                        Note = hoadon.GhiChu,
                        ShippingFee = 0,
                        Items = gioHang.Select(item => new CartItem
                        {
                            MaHh = item.MaHh,
                            TenHH = item.TenHH,
                            DonGia = item.DonGia,
                            Hinh = item.Hinh,
                            SoLuong = item.SoLuong
                        }).ToList()
                    };

                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                    return View("Success", orderSummary);
                }
                catch
                {
                    db.Database.RollbackTransaction();
                }
            }

            return View(Cart);
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item == null)
            {
                return NotFound();
            }

            if (quantity < 1)
            {
                quantity = 1;
            }

            item.SoLuong = quantity;
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);

            return Json(new
            {
                itemTotal = item.ThanhTien,
                cartSubtotal = gioHang.Sum(p => p.ThanhTien),
                cartQuantity = gioHang.Sum(p => p.SoLuong)
            });
        }
    }
}
