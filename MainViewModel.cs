using MOMO_QR_DANANG.Services; // Đảm bảo dòng này có mặt
using Newtonsoft.Json;
using RestSharp;
using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Forms; // Vẫn cần cho OpenFileDialog (WinForms)

namespace MOMO_QR_DANANG.ViewModels
{
    // --- BASE VIEWMODEL ---
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    // --- END BASE VIEWMODEL ---


    public class MainViewModel : BaseViewModel
    {
        // Properties để binding với UI
        private string _soTaiKhoan = "000123456789";
        public string SoTaiKhoan
        {
            get => _soTaiKhoan;
            set { _soTaiKhoan = value; OnPropertyChanged(); ((RelayCommand)GenerateQRCommand).RaiseCanExecuteChanged(); }
        }

        private string _soTien = "50000";
        public string SoTien
        {
            get => _soTien;
            set { _soTien = value; OnPropertyChanged(); ((RelayCommand)GenerateQRCommand).RaiseCanExecuteChanged(); }
        }

        private string _noiDung = "Thanh toan don hang";
        public string NoiDung
        {
            get => _noiDung;
            set { _noiDung = value; OnPropertyChanged(); }
        }

        private string _tenTaiKhoan = "NGUYEN VAN A";
        public string TenTaiKhoan
        {
            get => _tenTaiKhoan;
            set { _tenTaiKhoan = value; OnPropertyChanged(); ((RelayCommand)GenerateQRCommand).RaiseCanExecuteChanged(); }
        }

        private BitmapImage _logoImage;
        public BitmapImage LogoImage
        {
            get => _logoImage;
            set { _logoImage = value; OnPropertyChanged(); }
        }

        private BitmapImage _qrCodeImage;
        public BitmapImage QRCodeImage
        {
            get => _qrCodeImage;
            set { _qrCodeImage = value; OnPropertyChanged(); }
        }

        // Commands để binding với Button
        public ICommand SelectLogoCommand { get; }
        public ICommand GenerateQRCommand { get; }

        public MainViewModel()
        {
            SelectLogoCommand = new RelayCommand(SelectLogo);
            GenerateQRCommand = new RelayCommand(GenerateQR, CanGenerateQR);
        }

        private void SelectLogo(object parameter)
        {
            // Sử dụng System.Windows.Forms.OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png)|*.png;",
                Title = "Chọn ảnh Logo",
                CheckFileExists = true,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // WPF BitmapImage dùng Uri
                LogoImage = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private bool CanGenerateQR(object parameter)
        {
            // Kiểm tra các trường bắt buộc
            return !string.IsNullOrWhiteSpace(SoTaiKhoan) &&
                   int.TryParse(SoTien, out int soTienInt) && soTienInt > 0 &&
                   !string.IsNullOrWhiteSpace(TenTaiKhoan);
        }

        private async void GenerateQR(object parameter)
        {
            if (!int.TryParse(SoTien, out int soTienInt)) return;

            string logoBase64 = "";
            if (LogoImage != null)
            {
                // Chuyển WPF Image sang System.Drawing.Image để tạo Base64
                var drawingImage = APIRequestHelper.ImageSourceToDrawingImage(LogoImage);
                if (drawingImage != null)
                {
                    // Lấy Base64 prefix (data:image/png;base64,...)
                    logoBase64 = APIRequestHelper.ImageToBase64(drawingImage, ImageFormat.Png);
                }
            }

            var apiRequest = new APIRequest
            {
                acqId = 970436, // VCB
                accountNo = SoTaiKhoan.Trim(),
                accountName = TenTaiKhoan.Trim(),
                amount = soTienInt,
                logo = logoBase64,
                addInfo = NoiDung.Trim(),
                template = "compact2",
                theme = "compact2"
            };

            var jsonRequest = JsonConvert.SerializeObject(apiRequest);

            // Cấu hình