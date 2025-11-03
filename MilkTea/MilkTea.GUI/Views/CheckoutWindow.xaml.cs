using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MilkTea.DAL.Models;
using MilkTea.BLL.Services;
using MilkTea.BLL;
using MilkTea.GUI.Utils;

namespace MilkTea.GUI.Views
{
    public partial class CheckoutWindow : Window
    {
        private readonly List<CartItem> _cartItems;
        private readonly DiscountService _discountService;
        private readonly InvoiceService _invoiceService;
        private Discount? _appliedDiscount;

        // Totals
        private decimal _subtotal;
        private decimal _vat;
        private decimal _discountAmount;
        private decimal _total;

        public CheckoutWindow(List<CartItem> cartItems)
        {
            InitializeComponent();
            _cartItems = cartItems ?? new List<CartItem>();
            _discountService = new DiscountService();
            _invoiceService = new InvoiceService();

            LoadCartItems();
            CalculateTotals();
        }

        private void LoadCartItems()
        {
            itemsCart.ItemsSource = _cartItems;
        }

        private void CalculateTotals()
        {
            // Subtotal
            _subtotal = _cartItems.Sum(item => item.Subtotal);
            txtSubtotal.Text = CurrencyHelper.FormatVND(_subtotal);

            // VAT (10%)
            _vat = _subtotal * MilkTea.BLL.AppConfig.VATRate;
            txtVAT.Text = CurrencyHelper.FormatVND(_vat);

            // Discount
            _discountAmount = 0;
            if (_appliedDiscount != null)
            {
                _discountAmount = _subtotal * (_appliedDiscount.Percentage / 100);
            }
            txtDiscount.Text = CurrencyHelper.FormatVND(_discountAmount);

            // Total
            _total = _subtotal + _vat - _discountAmount;
            txtTotal.Text = CurrencyHelper.FormatVND(_total);
        }

        private void BtnApplyDiscount_Click(object sender, RoutedEventArgs e)
        {
            string code = txtDiscountCode.Text.Trim();
            
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Vui lòng nhập mã giảm giá!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var discount = _discountService.GetValidDiscountByCode(code);
            
            if (discount == null)
            {
                MessageBox.Show("Mã giảm giá không hợp lệ hoặc đã hết hạn!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtDiscountStatus.Text = "Mã không hợp lệ";
                _appliedDiscount = null;
            }
            else
            {
                _appliedDiscount = discount;
                txtDiscountStatus.Text = $"Đã áp dụng: {discount.Code} (-{discount.Percentage}%)";
                MessageBox.Show($"Đã áp dụng mã giảm giá {discount.Code} (-{discount.Percentage}%)", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            CalculateTotals();
        }

        private void BtnCash_Click(object sender, RoutedEventArgs e)
        {
            ProcessPayment("Tiền mặt", null);
        }

        private void BtnTransfer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Generate QR payment data
                string qrData = $"MOMO|{MilkTea.GUI.Utils.AppConfig.CurrentUser?.Username ?? "Admin"}|{_total}|Thanh toan don hang {DateTime.Now:ddMMyyyyHHmmss}";
                
                // Show QR Payment Window
                var qrWindow = new QRPaymentWindow(qrData, _total);
                if (qrWindow.ShowDialog() == true && qrWindow.IsPaymentConfirmed)
                {
                    ProcessPayment("Chuyển khoản", qrData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị QR:\n{ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessPayment(string paymentMethod, string? qrData)
        {
            try
            {
                if (_cartItems == null || _cartItems.Count == 0)
                {
                    MessageBox.Show("Giỏ hàng trống!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Create invoice
                int invoiceId = _invoiceService.CreateInvoice(
                    _cartItems,
                    _appliedDiscount?.DiscountId,
                    paymentMethod,
                    qrData
                );

                // Ask to export PDF
                var exportPdf = MessageBox.Show(
                    $"✅ Thanh toán thành công!\n\nMã hóa đơn: {invoiceId}\nPhương thức: {paymentMethod}\nTổng tiền: {CurrencyHelper.FormatVND(_total)}\n\nBạn có muốn xuất hóa đơn PDF không?",
                    "Thành công", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);

                if (exportPdf == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Get full invoice with details
                        var invoice = _invoiceService.GetInvoiceById(invoiceId);
                        if (invoice != null && invoice.InvoiceDetails != null)
                        {
                            string pdfPath = PdfExporter.GetDefaultInvoicePath(invoiceId);
                            PdfExporter.ExportInvoice(invoice, invoice.InvoiceDetails.ToList(), pdfPath);
                            
                            MessageBox.Show($"📄 Đã xuất hóa đơn PDF:\n{pdfPath}", "Thành công",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xuất PDF: {ex.Message}", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thanh toán: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
