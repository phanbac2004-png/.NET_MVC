using System;
using System.Collections.Generic;
using System.Linq;

namespace PhanXuanBac_22103100089.ViewModels
{
	public class OrderSuccessVM
	{
		public int OrderId { get; set; }
		public string? CustomerName { get; set; }
		public string? Address { get; set; }
		public string? Phone { get; set; }
		public DateTime OrderDate { get; set; }
		public string? Note { get; set; }
		public double ShippingFee { get; set; } = 0;
		public List<CartItem> Items { get; set; } = new List<CartItem>();

		public double Subtotal => Items.Sum(p => p.ThanhTien);
		public double Total => Subtotal + ShippingFee;
	}
}

