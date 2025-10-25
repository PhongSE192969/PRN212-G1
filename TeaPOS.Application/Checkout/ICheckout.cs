using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaPOS.Application.Checkout
{
    public interface ICheckout
    {
        void AddOrUpdateItem(CartItem item);
        IReadOnlyList<CartItem> GetItems();
        void ClearCart();

        bool ApplyDiscountCode(string? code);
        int? GetDiscountId();

        CheckoutTotals GetTotals();
    }
}

