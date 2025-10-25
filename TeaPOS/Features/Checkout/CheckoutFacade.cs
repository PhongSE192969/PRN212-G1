using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeaPOS.Application.Checkout;

namespace TeaPOS.Presentation.Wpf.Features.Checkout
{
    public sealed class CheckoutFacade : ICheckout
    {
        private readonly CheckoutViewModel _vm;
        public CheckoutFacade(CheckoutViewModel vm) => _vm = vm;

        public void AddOrUpdateItem(CartItem item) => _vm.AddOrUpdateItem(item);
        public IReadOnlyList<CartItem> GetItems() => _vm.Items.ToList();
        public void ClearCart() => _vm.Items.Clear();

        public bool ApplyDiscountCode(string? code)
        {
            _vm.DiscountCodeInput = code ?? "";
            _vm.ApplyDiscount();
            return _vm.DiscountID.HasValue;
        }

        public int? GetDiscountId() => _vm.DiscountID;
        public CheckoutTotals GetTotals() => _vm.Totals;
    }
}
