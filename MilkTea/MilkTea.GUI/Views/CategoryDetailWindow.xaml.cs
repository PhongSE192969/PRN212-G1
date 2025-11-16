using System.Windows;
using MilkTea.BLL.Services;
using MilkTea.DAL.Models;

namespace MilkTea.GUI.Views
{
    public partial class CategoryDetailWindow : Window
    {
        private readonly CategoryService _categoryService;
        private readonly Category? _category;
        private readonly bool _isEditMode;

        public CategoryDetailWindow()
        {
            InitializeComponent();
            _categoryService = new CategoryService();
            _isEditMode = false;
            txtTitle.Text = "Thêm danh mục";
            pnlCategoryId.Visibility = Visibility.Collapsed;
        }

        public CategoryDetailWindow(Category category)
        {
            InitializeComponent();
            _categoryService = new CategoryService();
            _category = category;
            _isEditMode = true;
            txtTitle.Text = "Sửa danh mục";
            pnlCategoryId.Visibility = Visibility.Visible;
            
            
            txtCategoryId.Text = category.CategoryId.ToString();
            txtCategoryName.Text = category.CategoryName;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên danh mục!", 
                    "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCategoryName.Focus();
                return;
            }

            try
            {
                if (_isEditMode)
                {
                    
                    var category = new Category
                    {
                        CategoryId = _category!.CategoryId,
                        CategoryName = txtCategoryName.Text.Trim()
                    };

                    if (_categoryService.UpdateCategory(category))
                    {
                        MessageBox.Show("Cập nhật danh mục thành công!", 
                            "Thanh cong", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Không thể cập nhật danh mục!", 
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    
                    var category = new Category
                    {
                        CategoryName = txtCategoryName.Text.Trim()
                    };

                    if (_categoryService.CreateCategory(category))
                    {
                        MessageBox.Show("Thêm danh mục thành công!", 
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Tên danh mục đã tồn tại hoặc không thể thêm danh mục!", 
                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu danh mục:\n{ex.Message}", 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
