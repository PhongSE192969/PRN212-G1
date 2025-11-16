using System;
using System.Linq;
using System.Windows;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using MilkTea.DAL.Data;
using MilkTea.BLL.Services;

namespace MilkTea.GUI.Views
{
    public partial class RevenueChartWindow : Window
    {
        private readonly RevenueService _revenueService;

        public RevenueChartWindow()
        {
            InitializeComponent();
            _revenueService = new RevenueService();
            LoadChart();
        }

        private void LoadChart()
        {
            try
            {
                using (var context = new TeaPOSDbContext())
                {
                    // Lấy dữ liệu doanh thu theo tháng từ ViewMonthlyRevenue
                    var data = context.ViewMonthlyRevenues
                        .OrderBy(v => v.Year)
                        .ThenBy(v => v.Month)
                        .Select(v => new
                        {
                            Label = $"{v.Month}/{v.Year}",
                            Year = v.Year ?? DateTime.Now.Year,
                            Month = v.Month ?? 1,
                            TotalRevenue = v.TotalRevenue ?? 0
                        })
                        .ToList();

                    if (!data.Any())
                    {
                        MessageBox.Show("Chưa có dữ liệu doanh thu!", "Thông báo", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    // Cập nhật period
                    var latestYear = data.Max(d => d.Year);
                    txtPeriod.Text = $"Năm {latestYear}";

                    // Tạo PlotModel
                    var model = new PlotModel
                    {
                        Title = "Doanh Thu Theo Tháng",
                        TitleFontSize = 18,
                        DefaultColors = new[] { OxyColor.FromRgb(255, 107, 53) } // #FF6B35
                    };

                    // Thêm trục Category (tháng) - phải ở LEFT cho BarSeries
                    var categoryAxis = new CategoryAxis
                    {
                        Position = AxisPosition.Left,
                        Title = "Tháng",
                        Key = "MonthAxis",
                        ItemsSource = data.Select(d => d.Label).ToList()
                    };
                    model.Axes.Add(categoryAxis);

                    // Thêm trục Value (doanh thu) - phải ở BOTTOM cho BarSeries
                    var valueAxis = new LinearAxis
                    {
                        Position = AxisPosition.Bottom,
                        Title = "Doanh Thu (VNĐ)",
                        Key = "RevenueAxis",
                        StringFormat = "#,##0",
                        MinimumPadding = 0.1,
                        MaximumPadding = 0.1
                    };
                    model.Axes.Add(valueAxis);

                    // Tạo BarSeries
                    var barSeries = new BarSeries
                    {
                        Title = "Doanh Thu",
                        FillColor = OxyColor.FromRgb(255, 107, 53),
                        StrokeColor = OxyColors.White,
                        StrokeThickness = 1,
                        ItemsSource = data.Select((d, index) => new BarItem
                        {
                            Value = (double)d.TotalRevenue,
                            CategoryIndex = index
                        }).ToList()
                    };
                    model.Series.Add(barSeries);

                    // Gán model cho chart
                    chartRevenue.Model = model;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải biểu đồ: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadChart();
            MessageBox.Show("Đã làm mới biểu đồ!", "Thông báo", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDetailReport_Click(object sender, RoutedEventArgs e)
        {
            var reportWindow = new RevenueReportWindow();
            reportWindow.Show();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
