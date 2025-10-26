using System.Linq;
using System.Windows;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using MilkTeaApp.Report.Kian.Models;

namespace MilkTeaApp.Report.Kian
{
    public partial class RevenueChartWindow : Window
    {
        public RevenueChartWindow()
        {
            InitializeComponent();
            LoadChart();
        }

        private void LoadChart()
        {
            try
            {
                using (var context = new TeaPosContext())
                {
                    // 🔹 Lấy dữ liệu doanh thu theo tháng
                    var data = context.Revenues
                        .Where(r => r.ReportDate.HasValue)
                        .AsEnumerable()
                        .GroupBy(r => new { Year = r.ReportDate.Value.Year, Month = r.ReportDate.Value.Month })
                        .Select(g => new
                        {
                            Thang = $"{g.Key.Month}/{g.Key.Year}",
                            TongDoanhThu = g.Sum(e => e.TotalRevenue)
                        })
                        .OrderBy(e => e.Thang)
                        .ToList();

                    // 🔹 Khởi tạo biểu đồ
                    var model = new PlotModel
                    {
                        Title = "Doanh thu theo tháng",
                        TitleFontSize = 18,
                        TextColor = OxyColors.Brown,
                        PlotAreaBorderColor = OxyColors.Gray
                    };

                    // 🔹 Series (Bar chart)
                    var barSeries = new BarSeries
                    {
                        Title = "Doanh thu (VNĐ)",
                        FillColor = OxyColors.SandyBrown,
                        StrokeColor = OxyColors.Brown,
                        StrokeThickness = 1
                    };

                    foreach (var item in data)
                    {
                        barSeries.Items.Add(new BarItem(item.TongDoanhThu ?? 0));
                    }

                    // 🔹 Trục Y (Tháng) - CategoryAxis
                    var categoryAxis = new CategoryAxis
                    {
                        Position = AxisPosition.Left,
                        ItemsSource = data,
                        LabelField = "Thang",
                        Title = "Tháng",
                        TextColor = OxyColors.Brown,
                        AxislineColor = OxyColors.Gray
                    };

                    // 🔹 Trục X (Doanh thu)
                    var valueAxis = new LinearAxis
                    {
                        Position = AxisPosition.Bottom,
                        Title = "Doanh thu (VNĐ)",
                        TextColor = OxyColors.Brown,
                        AxislineColor = OxyColors.Gray,
                        MajorGridlineColor = OxyColor.FromRgb(230, 230, 230),
                        MajorGridlineStyle = LineStyle.Solid
                    };

                    // 🔹 Thêm vào model
                    model.Series.Add(barSeries);
                    model.Axes.Add(categoryAxis);
                    model.Axes.Add(valueAxis);

                    // 🔹 Gán vào PlotView
                    RevenuePlot.Model = model;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu biểu đồ: " + ex.Message,
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
