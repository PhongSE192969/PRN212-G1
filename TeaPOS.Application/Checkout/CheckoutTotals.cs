using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeaPOS.Application.Checkout;

public sealed class CheckoutTotals : INotifyPropertyChanged
{
    public decimal Subtotal { get; private set; }
    public decimal VatAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal GrandTotal => Subtotal + VatAmount - DiscountAmount;

    public void Recalculate(IEnumerable<CartItem> items, decimal vatRate, decimal? percentDiscount)
    {
        Subtotal = items.Sum(i => i.LineSubtotal);
        VatAmount = Math.Round(Subtotal * vatRate, 0, MidpointRounding.AwayFromZero);
        DiscountAmount = percentDiscount.HasValue
            ? Math.Round(Subtotal * (percentDiscount.Value / 100m), 0, MidpointRounding.AwayFromZero)
            : 0m;

        Changed(nameof(Subtotal));
        Changed(nameof(VatAmount));
        Changed(nameof(DiscountAmount));
        Changed(nameof(GrandTotal));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void Changed(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}
