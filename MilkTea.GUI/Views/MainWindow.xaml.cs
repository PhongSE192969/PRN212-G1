using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MilkTea.DAL.Models;
using MilkTea.GUI.Utils;
using MilkTea.GUI.ViewModels;
using MilkTea.BLL.Services;

namespace MilkTea.GUI.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly CategoryService _categoryService = new CategoryService();
        private readonly ProductService _productService = new ProductService();

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                _viewModel = new MainViewModel();

                // Set delete button visibility according to role using FindName to avoid generated field dependency
                var delBtn = this.FindName("btnDeleteCategory") as Button;
                if (delBtn != null)
                    delBtn.Visibility = AppConfig.IsAdmin ? Visibility.Visible : Visibility.Collapsed;

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

        // New: delete selected category with confirmation. Only available for Admins.
        private void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (!AppConfig.IsAdmin)
            {
                MessageBox.Show("Bạn không có quyền xóa danh mục.", "Quyền", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = _viewModel.SelectedCategory;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục để xóa (nhấn nút danh mục).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // If category has products, prevent accidental deletion
            if (selected.Products != null && selected.Products.Any())
            {
                var res = MessageBox.Show($"Danh mục '{selected.CategoryName}' có {selected.Products.Count} sản phẩm. Bạn có chắc muốn xóa? Tất cả sản phẩm sẽ được xóa.", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res != MessageBoxResult.Yes) return;
            }
            else
            {
                var res = MessageBox.Show($"Bạn có chắc muốn xóa danh mục '{selected.CategoryName}'?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res != MessageBoxResult.Yes) return;
            }

            try
            {
                var success = _categoryService.DeleteCategory(selected.CategoryId);
                if (success)
                {
                    // Refresh categories in viewmodel
                    var categories = _categoryService.GetAllCategories();
                    _viewModel.Categories.Clear();
                    foreach (var c in categories)
                        _viewModel.Categories.Add(c);

                    // Reset selected category and reload products
                    _viewModel.SelectedCategory = null;
                    var prods = _productService.GetAllProducts();
                    _viewModel.Products.Clear();
                    foreach (var p in prods)
                        _viewModel.Products.Add(p);

                    MessageBox.Show("Xóa danh mục thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể xóa danh mục. Vui lòng kiểm tra quyền hoặc kết nối DB.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Exception ex)
            {
                // Show detailed error to help diagnose the problem
                var message = $"Không thể xóa danh mục. Lỗi: {ex.Message}";
                if (ex.InnerException != null)
                    message += $"\nNguyên nhân chi tiết: {ex.InnerException.Message}";

                MessageBox.Show(message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
