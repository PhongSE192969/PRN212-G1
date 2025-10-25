using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaPOS.Application.Checkout
{
        public class CartItem
        {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }

        public int? ToppingID { get; set; }
        public string? ToppingName { get; set; }

        public decimal ProductPrice { get; set; }
        public decimal ToppingPrice { get; set; }

        public decimal UnitPrice => ProductPrice + ToppingPrice;
        public decimal LineSubtotal => UnitPrice * Quantity;
        }
}
