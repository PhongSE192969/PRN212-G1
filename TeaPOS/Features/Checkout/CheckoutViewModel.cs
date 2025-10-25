using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeaPOS.Application.Checkout;
using TeaPOS.Application.Common.DTOs;
using TeaPOS.Infrastructure.Services;

namespace TeaPOS.Presentation.Wpf.Features.Checkout
{
    public sealed class CheckoutViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CartItem> Items { get; } = new();
        public CheckoutTotals Totals { get; } = new();

        private readonly IDiscountService _discountService;
        private DiscountDto? _appliedDiscount;

        public CheckoutViewModel(IDiscountService discountService)
        {
            _discountService = discountService;
            VatRate = 0.10m;
        }

        public decimal VatRate { get; set; }
        public string DiscountCodeInput { get; set; } = "";

        public string AppliedDiscountText =>
            _appliedDiscount is null ? "Không áp mã" : $"{_appliedDiscount.Code} ({_appliedDiscount.Percentage:#0}%)";
        public int? DiscountID => _appliedDiscount?.DiscountID;

        public void ApplyDiscount()
        {
            _appliedDiscount = string.IsNullOrWhiteSpace(DiscountCodeInput)
                ? null
                : _discountService.GetValidDiscountByCode(DiscountCodeInput);
            RefreshTotals();
        }

        public void RefreshTotals()
        {
            Totals.Recalculate(Items, VatRate, _appliedDiscount?.Percentage);
            PropertyChanged?.Invoke(this, new(nameof(AppliedDiscountText)));
        }

        public void AddOrUpdateItem(CartItem item)
        {
            var same = Items.FirstOrDefault(x => x.ProductID == item.ProductID && x.ToppingID == item.ToppingID && x.UnitPrice == item.UnitPrice);
            if (same is not null) same.Quantity += item.Quantity;
            else Items.Add(item);
            RefreshTotals();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
