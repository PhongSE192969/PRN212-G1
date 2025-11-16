using System.Windows;
using MilkTea.BLL.Services;
using MilkTea.DAL.Models;

namespace MilkTea.GUI.Views
{
    public partial class ToppingDetailDialog : Window
    {
        private readonly ToppingService _toppingService = new ToppingService();
        private Topping? _topping;
        private bool _isEditMode;

        public ToppingDetailDialog()
        {
            InitializeComponent();
            _isEditMode = false;
            txtTitle.Text = "THÊM TOPPING MỚI";
        }

        public ToppingDetailDialog(Topping topping)
        {
            InitializeComponent();
            _isEditMode = true;
            _topping = topping;
            txtTitle.Text = "SỬA TOPPING";
            LoadToppingData();
        }

        private void LoadToppingData()
        {
            if (_topping == null) return;

            txtToppingName.Text = _topping.ToppingName;
            txtPrice.Text = _topping.Price.ToString();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtToppingName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên topping!", "Lỗi",
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

            var topping = _topping ?? new Topping();
            topping.ToppingName = txtToppingName.Text.Trim();
            topping.Price = decimal.Parse(txtPrice.Text);

            bool success = _isEditMode
                ? _toppingService.UpdateTopping(topping)
                : _toppingService.CreateTopping(topping);

            if (!success)
            {
                MessageBox.Show("Lưu topping thất bại!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Lưu topping thành công!", "Thành công",
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