using System.Windows;
using System.Windows.Controls;
using MilkTea.BLL.Services;
using MilkTea.DAL.Models;

namespace MilkTea.GUI.Views
{
    public partial class CategoryPage : Window
    {
        private readonly CategoryService _categoryService;
        private Category? _selectedCategory;

        public CategoryPage()
        {
            InitializeComponent();
            _categoryService = new CategoryService();
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                var categories = _categoryService.GetAllCategories();
                dgCategories.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh mục:\n{ex.Message}", 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedCategory = dgCategories.SelectedItem as Category;
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            var detailWindow = new CategoryDetailWindow();
            if (detailWindow.ShowDialog() == true)
            {
                LoadCategories();
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục trước khi sửa", 
                    "Thong bao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var detailWindow = new CategoryDetailWindow(_selectedCategory);
            if (detailWindow.ShowDialog() == true)
            {
                LoadCategories();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục trước khi xoá", 
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check if category has products
            string message;
            if (_selectedCategory.Products != null && _selectedCategory.Products.Count > 0)
            {
                message = $"Danh mục '{_selectedCategory.CategoryName}' còn {_selectedCategory.Products.Count} sản phẩm.\n" +
                         "Bạn có chắc chắn muốn xoá không?";
            }
            else
            {
                message = $"Bạn có chắc chắn xoá danh mục '{_selectedCategory.CategoryName}'?";
            }

            var result = MessageBox.Show(
                message,
                "Xác nhận xoá danh mục",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (_categoryService.DeleteCategory(_selectedCategory.CategoryId))
                    {
                        MessageBox.Show("Xoá danh mục thành công!", 
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadCategories();
                    }
                    else
                    {
                        MessageBox.Show("Không thể xoá danh mục!", 
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xoá danh mục:\n{ex.Message}", 
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
