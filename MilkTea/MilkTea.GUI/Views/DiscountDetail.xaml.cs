using System;
using System.Windows;
using MilkTea.BLL.Services;
using MilkTea.DAL.Models;

namespace MilkTea.GUI.Views
{
    public partial class DiscountDetail : Window
    {
        private readonly DiscountService _discountService = new DiscountService();
        private readonly Discount _discount;
        private readonly bool _isEdit;

        // MODE THÊM
        public DiscountDetail()
        {
            InitializeComponent();
            _discount = new Discount();
            _isEdit = false;
            SetupMode();
        }

        // MODE SỬA (nhận Discount từ màn list)
        public DiscountDetail(Discount discount)
        {
            InitializeComponent();
            _discount = new Discount
            {
                DiscountId = discount.DiscountId,
                Code = discount.Code,
                Description = discount.Description,
                Percentage = discount.Percentage,
                ExpireDate = discount.ExpireDate
            };
            _isEdit = true;
            SetupMode();
            LoadDataToForm();
        }

        private void SetupMode()
        {
            if (_isEdit)
            {
                Title = "Sửa mã giảm giá";
                btnAdd.Visibility = Visibility.Collapsed;
                btnUpdate.Visibility = Visibility.Visible;
            }
            else
            {
                Title = "Thêm mã giảm giá";
                btnAdd.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadDataToForm()
        {
            txtCode.Text = _discount.Code;
            txtDescription.Text = _discount.Description;
            txtPercentage.Text = _discount.Percentage.ToString();
            dpExpireDate.SelectedDate = _discount.ExpireDate;
        }

        private bool ValidateAndFillDiscount()
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                MessageBox.Show("Mã giảm giá không được trống.",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(txtPercentage.Text, out var percent) || percent <= 0)
            {
                MessageBox.Show("Phần trăm giảm phải là số > 0.",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            _discount.Code = txtCode.Text.Trim();
            _discount.Description = string.IsNullOrWhiteSpace(txtDescription.Text)
                ? null
                : txtDescription.Text.Trim();
            _discount.Percentage = percent;
            _discount.ExpireDate = dpExpireDate.SelectedDate;

            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateAndFillDiscount()) return;

            if (!_discountService.CreateDiscount(_discount))
            {
                MessageBox.Show("Thêm mã giảm giá thất bại (có thể trùng mã).",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateAndFillDiscount()) return;

            if (!_discountService.UpdateDiscount(_discount))
            {
                MessageBox.Show("Cập nhật mã giảm giá thất bại.",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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
