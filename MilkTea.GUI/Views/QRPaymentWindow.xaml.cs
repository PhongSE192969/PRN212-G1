using System.Windows;
using MilkTea.GUI.Utils;

namespace MilkTea.GUI.Views
{
    public partial class QRPaymentWindow : Window
    {
        public bool IsPaymentConfirmed { get; private set; }
        private readonly decimal _amount;

        public QRPaymentWindow(string qrData, decimal amount)
        {
            InitializeComponent();
            
            _amount = amount;
            
            // Display amount
            txtAmount.Text = $"Tổng tiền: {CurrencyHelper.FormatVND(amount)}";
            
            // Generate QR code using MoMo API
            GenerateMoMoQR(amount);
            
            IsPaymentConfirmed = false;
        }

        private async void GenerateMoMoQR(decimal amount)
        {
            try
            {
                // Hiển thị loading
                txtAmount.Text = "Đang tạo mã QR...";
                
                // Gọi MoMo API giống legacy code
                var qrImage = await System.Threading.Tasks.Task.Run(() => 
                    MoMoAPIHelper.GenerateMoMoQR(
                        accountNo: "1031177625", // Số điện thoại MoMo
                        accountName: "TEAPOS QUAN TRA SUA",
                        amount: amount,
                        description: $"Thanh toan don hang {DateTime.Now:ddMMyyyyHHmmss}",
                        logoBase64: "" // Để trống, có thể thêm logo sau
                    ));

                if (qrImage != null)
                {
                    imgQRCode.Source = qrImage;
                    txtAmount.Text = $"Tổng tiền: {CurrencyHelper.FormatVND(amount)}";
                }
                else
                {
                    // Fallback: dùng QRCoder nếu API lỗi
                    UseFallbackQR(amount);
                    MessageBox.Show("Không thể tạo QR MoMo. Sử dụng QR code đơn giản thay thế.", 
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                // Fallback: dùng QRCoder nếu có exception
                UseFallbackQR(amount);
                MessageBox.Show($"Không thể kết nối MoMo API.\n{ex.Message}\n\nSử dụng QR code đơn giản thay thế.", 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UseFallbackQR(decimal amount)
        {
            // Dùng QRCoder đơn giản như backup
            string qrData = $"MOMO|{AppConfig.CurrentUser?.Username ?? "Admin"}|{amount}|Thanh toan don hang {DateTime.Now:ddMMyyyyHHmmss}";
            var qrImage = QRCodeHelper.GenerateQRCode(qrData, 20);
            imgQRCode.Source = qrImage;
            txtAmount.Text = $"Tổng tiền: {CurrencyHelper.FormatVND(amount)}";
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            IsPaymentConfirmed = true;
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            IsPaymentConfirmed = false;
            DialogResult = false;
            Close();
        }
    }
}
