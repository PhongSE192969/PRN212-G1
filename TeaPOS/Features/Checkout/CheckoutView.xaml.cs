using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TeaPOS.Application.Checkout;
using TeaPOS.Infrastructure.Services;

namespace TeaPOS.Presentation.Wpf.Features.Checkout
{
    /// <summary>
    /// Interaction logic for CheckoutView.xaml
    /// </summary>
    public partial class CheckoutView : Window
    {
        public CheckoutViewModel VM { get; }

        private void OpenPaymentQr_Click(object sender, RoutedEventArgs e)
        {
            // TODO: mở cửa sổ QR nếu đã có
            // new PaymentQrWindow().ShowDialog();
            MessageBox.Show("TODO: Mở màn Thanh toán (QR)", "Thông báo");
        }

        private void OpenPrint_Click(object sender, RoutedEventArgs e)
        {
            // TODO: mở cửa sổ In/PDF nếu đã có
            // if (VM.LastInvoiceId.HasValue) new InvoicePrintWindow(VM.LastInvoiceId.Value).Show();
            // else MessageBox.Show("Chưa có hoá đơn để in.");

            MessageBox.Show("TODO: Mở màn In / Xuất PDF", "Thông báo");
        }
        public CheckoutView()
        {
            InitializeComponent();
            VM = new CheckoutViewModel(new DiscountService());
            DataContext = VM;

            // Demo dữ liệu để test nhanh
#if DEBUG
            // DEMO chỉ chạy khi build Debug
            VM.AddOrUpdateItem(new CartItem
            {
                ProductID = 1,
                ProductName = "Trà sữa trân châu",
                Quantity = 1,
                ProductPrice = 35000,
                ToppingPrice = 5000,
                ToppingName = "Trân châu đen"
            });
#endif
        }


        private void ApplyDiscount_Click(object sender, RoutedEventArgs e) => VM.ApplyDiscount();
    }
}
