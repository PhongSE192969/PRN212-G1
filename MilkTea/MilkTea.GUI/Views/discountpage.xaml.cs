using System.Windows;
using MilkTea.BLL.Services;
using MilkTea.DAL.Models;

namespace MilkTea.GUI.Views
{
    public partial class discountpage : Window
    {
        private readonly DiscountService _discountService = new DiscountService();

        public discountpage()
        {
            InitializeComponent();
            LoadDiscounts();
        }

        private void LoadDiscounts()
        {
            dgDiscounts.ItemsSource = _discountService.GetAllDiscounts();
        }

        private Discount? GetSelectedDiscount()
        {
            return dgDiscounts.SelectedItem as Discount;
        }

        // MỞ DETAIL Ở CHẾ ĐỘ THÊM
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new DiscountDetail();        // mode: Add
            win.Owner = this;

            if (win.ShowDialog() == true)
            {
                LoadDiscounts();                  // reload list sau khi thêm
            }
        }

        // MỞ DETAIL Ở CHẾ ĐỘ SỬA
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedDiscount();
            if (selected == null)
            {
                MessageBox.Show("Chọn 1 dòng để sửa.",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // truyền bản ghi đang chọn sang Detail để sửa
            var win = new DiscountDetail(selected);
            win.Owner = this;

            if (win.ShowDialog() == true)
            {
                LoadDiscounts();                  // reload sau khi lưu
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedDiscount();
            if (selected == null)
            {
                MessageBox.Show("Chọn 1 dòng để xóa.",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Xóa mã '{selected.Code}' ?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            if (!_discountService.DeleteDiscount(selected.DiscountId))
            {
                MessageBox.Show("Xóa thất bại.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadDiscounts();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            // Tạm thời để TODO, sau muốn thì làm export/print thật
            MessageBox.Show("Chức năng In chưa được triển khai.",
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
