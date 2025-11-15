using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MilkTea.DAL.Models;
using MilkTea.GUI.Utils;
using MilkTea.GUI.ViewModels;

namespace MilkTea.GUI.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                _viewModel = new MainViewModel();
                
                LoadData();
                UpdateUI();
                
                // Display user info
                if (AppConfig.CurrentUser != null)
                {
                    txtWelcome.Text = $"Xin chào, {AppConfig.CurrentUser.FullName} ({AppConfig.CurrentUser.Role})";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi tạo ứng dụng:\n{ex.Message}\n\nChi tiết:\n{ex.InnerException?.Message}", 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void LoadData()
        {
            lstCategories.ItemsSource = _viewModel.Categories;
            lstProducts.ItemsSource = _viewModel.Products;
            lstCart.ItemsSource = _viewModel.CartItems;
        }

        private void UpdateUI()
        {
            txtSubtotal.Text = $"{_viewModel.TotalAmount:N0} đ";
            txtVAT.Text = $"{_viewModel.VAT:N0} đ";
            txtDiscount.Text = $"{_viewModel.DiscountAmount:N0} đ";
            txtTotal.Text = $"{_viewModel.FinalAmount:N0} đ";
        }

        private void BtnCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int categoryId)
            {
                var category = _viewModel.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
                _viewModel.SelectedCategory = category;
            }
        }

        private void ProductCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is int productId)
            {
                var product = _viewModel.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    ShowAddToCartDialog(product);
                }
            }
        }

        private void ShowAddToCartDialog(Product product)
        {
            try
            {
                var dialog = new AddToCartDialog(product, _viewModel.Toppings.ToList());
                if (dialog.ShowDialog() == true)
                {
                    _viewModel.AddToCart(
                        dialog.SelectedProductId,
                        dialog.Quantity,
                        dialog.SelectedToppingId
                    );
                    UpdateUI();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm vào giỏ hàng:\n{ex.Message}\n\nChi tiết:\n{ex.InnerException?.Message}", 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CartItem item)
            {
                _viewModel.RemoveFromCart(item);
                UpdateUI();
            }
        }

        private void BtnApplyDiscount_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DiscountCode = txtDiscountCode.Text;
            
            if (_viewModel.ApplyDiscountCode())
            {
                txtDiscountInfo.Text = $"✓ Áp dụng mã {_viewModel.AppliedDiscount?.Code} - Giảm {_viewModel.AppliedDiscount?.Percentage}%";
                txtDiscountInfo.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                txtDiscountInfo.Text = "✗ Mã giảm giá không hợp lệ hoặc đã hết hạn";
                txtDiscountInfo.Foreground = System.Windows.Media.Brushes.Red;
            }
            
            UpdateUI();
        }

        private void BtnCheckout_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CartItems.Count == 0)
            {
                MessageBox.Show("Giỏ hàng trống!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var checkoutWindow = new CheckoutWindow(_viewModel.CartItems.ToList());
            if (checkoutWindow.ShowDialog() == true)
            {
                // Clear cart after successful payment
                _viewModel.ClearCart();
                UpdateUI();
            }
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            var reportWindow = new RevenueReportWindow();
            reportWindow.Show();
        }
        private void BtnInvoices_Click(object sender, RoutedEventArgs e)
        {
            var invoiceDetailWindow = new InvoiceDetailWindow();
            invoiceDetailWindow.Show();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                AppConfig.CurrentUser = null;
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

    }
}
