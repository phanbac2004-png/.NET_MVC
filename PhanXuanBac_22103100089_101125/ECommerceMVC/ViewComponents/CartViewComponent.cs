using PhanXuanBac_22103100089.Helpers;
using PhanXuanBac_22103100089.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace PhanXuanBac_22103100089.ViewComponents
{
	public class CartViewComponent : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			var cart = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

			return View("CartPanel", new CartModel
			{
				Quantity = cart.Sum(p => p.SoLuong),
				Total = cart.Sum(p => p.ThanhTien)
			});
		}
	}
}
