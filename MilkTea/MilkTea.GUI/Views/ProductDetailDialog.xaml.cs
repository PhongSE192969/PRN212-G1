using System.Windows;
using MilkTea.BLL.Services;
using MilkTea.DAL.Models;

namespace MilkTea.GUI.Views
{
    public partial class ProductDetailDialog : Window
    {
        private readonly ProductService _productService = new ProductService();
        private readonly CategoryService _categoryService = new CategoryService();
        private Product? _product;
        private bool _isEditMode;

        // Constructor for ADD mode
        public ProductDetailDialog()
        {
            InitializeComponent();
            _isEditMode = false;
            txtTitle.Text = "THÊM SẢN PHẨM MỚI";
            LoadCategories();
        }

        // Constructor for EDIT mode
        public ProductDetailDialog(Product product)
        {
            InitializeComponent();
            _isEditMode = true;
            _product = product;
            txtTitle.Text = "SỬA SẢN PHẨM";
            LoadCategories();
            LoadProductData();
        }

        private void LoadCategories()
        {
            cboCategory.ItemsSource = _categoryService.GetAllCategories();
        }

        private void LoadProductData()
        {
            if (_product == null) return;

            txtProductName.Text = _product.ProductName;
            cboCategory.SelectedValue = _product.CategoryId;
            txtPrice.Text = _product.Price.ToString();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Giá không hợp lệ!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            var product = _product ?? new Product();
            product.ProductName = txtProductName.Text.Trim();
            product.CategoryId = (int?)cboCategory.SelectedValue;
            product.Price = decimal.Parse(txtPrice.Text);

            bool success = _isEditMode
                ? _productService.UpdateProduct(product)
                : _productService.CreateProduct(product);

            if (!success)
            {
                MessageBox.Show("Lưu sản phẩm thất bại!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Lưu sản phẩm thành công!", "Thành công",
                MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}