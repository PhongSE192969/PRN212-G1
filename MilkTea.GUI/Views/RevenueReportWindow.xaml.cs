using System;
using System.Linq;
using System.Windows;
using MilkTea.BLL.Services;

namespace MilkTea.GUI.Views
{
    public partial class RevenueReportWindow : Window
    {
        private readonly RevenueService _revenueService;

        public RevenueReportWindow()
        {
            InitializeComponent();
            _revenueService = new RevenueService();
            LoadFilters();
            LoadReport();
        }

        private void LoadFilters()
        {
            // Load months
            for (int i = 1; i <= 12; i++)
            {
                cboMonth.Items.Add(i);
            }
            cboMonth.SelectedIndex = DateTime.Now.Month - 1;

            // Load years (last 5 years)
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear; i >= currentYear - 5; i--)
            {
                cboYear.Items.Add(i);
            }
            cboYear.SelectedIndex = 0;
        }

        private void LoadReport()
        {
            try
            {
                if (cboMonth.SelectedItem == null || cboYear.SelectedItem == null)
                    return;

                int month = (int)cboMonth.SelectedItem;
                int year = (int)cboYear.SelectedItem;

                // Lấy dữ liệu Revenue theo tháng/năm
                var revenues = _revenueService.GetRevenueByMonth(year, month);

                dgRevenue.ItemsSource = revenues;

                // Tính tổng
                if (revenues.Any())
                {
                    int totalDays = revenues.Count;
                    decimal totalRevenue = revenues.Sum(r => r.TotalRevenue);
                    decimal average = totalDays > 0 ? totalRevenue / totalDays : 0;

                    txtTotalInvoices.Text = totalDays.ToString("N0") + " ngày";
                    txtTotalRevenue.Text = $"{totalRevenue:N0} đ";
                    txtAverage.Text = $"{average:N0} đ";
                }
                else
                {
                    txtTotalInvoices.Text = "0 ngày";
                    txtTotalRevenue.Text = "0 đ";
                    txtAverage.Text = "0 đ";
                    MessageBox.Show($"Không có dữ liệu doanh thu cho tháng {month}/{year}", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void BtnChart_Click(object sender, RoutedEventArgs e)
        {
            var chartWindow = new RevenueChartWindow();
            chartWindow.Show();
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tính năng xuất Excel đang được phát triển!", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
