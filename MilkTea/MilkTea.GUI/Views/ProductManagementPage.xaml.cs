using System.Windows;
using MilkTea.BLL.Services;
using MilkTea.DAL.Models;

namespace MilkTea.GUI.Views
{
    public partial class ProductManagementPage : Window
    {
        private readonly ProductService _productService = new ProductService();
        private readonly ToppingService _toppingService = new ToppingService();

        public ProductManagementPage()
        {
            InitializeComponent();
            LoadProducts();
            LoadToppings();
        }

        private void LoadProducts()
        {
            dgProducts.ItemsSource = _productService.GetAllProducts();
        }

        private void LoadToppings()
        {
            dgToppings.ItemsSource = _toppingService.GetAllToppings();
        }

        // ========== PRODUCT CRUD ==========
        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProductDetailDialog();
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void BtnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgProducts.SelectedItem as Product;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new ProductDetailDialog(selected);
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void BtnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgProducts.SelectedItem as Product;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc muốn xóa sản phẩm '{selected.ProductName}'?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            if (!_productService.DeleteProduct(selected.ProductId))
            {
                MessageBox.Show("Không thể xóa sản phẩm! Có thể sản phẩm đã được sử dụng trong hóa đơn.",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Xóa sản phẩm thành công!", "Thành công",
                MessageBoxButton.OK, MessageBoxImage.Information);
            LoadProducts();
        }

        // ========== TOPPING CRUD ==========
        private void BtnAddTopping_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ToppingDetailDialog();
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                LoadToppings();
            }
        }

        private void BtnEditTopping_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgToppings.SelectedItem as Topping;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn topping cần sửa!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new ToppingDetailDialog(selected);
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                LoadToppings();
            }
        }

        private void BtnDeleteTopping_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgToppings.SelectedItem as Topping;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn topping cần xóa!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc muốn xóa topping '{selected.ToppingName}'?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            if (!_toppingService.DeleteTopping(selected.ToppingId))
            {
                MessageBox.Show("Không thể xóa topping! Có thể topping đã được sử dụng trong hóa đơn.",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Xóa topping thành công!", "Thành công",
                MessageBoxButton.OK, MessageBoxImage.Information);
            LoadToppings();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}