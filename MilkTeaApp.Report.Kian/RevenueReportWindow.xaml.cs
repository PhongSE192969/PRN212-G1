using System;
using System.Linq;
using System.Windows;
using MilkTeaApp.Report.Kian.Models;

namespace MilkTeaApp.Report.Kian
{
    public partial class RevenueReportWindow : Window
    {
        public RevenueReportWindow()
        {
            InitializeComponent();
            LoadRevenue();
        }

        // 🔹 Load danh sách doanh thu từ DB
        private void LoadRevenue()
        {
            try
            {
                using (var context = new TeaPosContext())
                {
                    var data = context.Revenues
                        .OrderByDescending(r => r.ReportDate)
                        .Select(r => new
                        {
                            Ngày = r.ReportDate,
                            DoanhThu = r.TotalRevenue
                        })
                        .ToList();

                    dgRevenue.ItemsSource = data;
                    txtStatus.Text = $"✅ Đã tải {data.Count} dòng doanh thu.";
                }
            }
            catch (Exception ex)
            {
                txtStatus.Text = "❌ Lỗi khi tải dữ liệu: " + ex.Message;
                txtStatus.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        // 🔹 Cập nhật doanh thu hôm nay
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new TeaPosContext())
                {
                    var today = DateTime.Today;
                    var todayDateOnly = DateOnly.FromDateTime(today);

                    // Tính tổng doanh thu hôm nay từ bảng Invoices
                    var total = context.Invoices
                        .Where(i => i.InvoiceDate.HasValue && i.InvoiceDate.Value.Date == today)
                        .Sum(i => (double?)i.FinalAmount) ?? 0;

                    // Cập nhật hoặc thêm vào Revenue
                    var record = context.Revenues.FirstOrDefault(r => r.ReportDate == todayDateOnly);
                    if (record != null)
                    {
                        record.TotalRevenue = total;
                    }
                    else
                    {
                        context.Revenues.Add(new Revenue
                        {
                            ReportDate = todayDateOnly,
                            TotalRevenue = total
                        });
                    }

                    context.SaveChanges();
                }

                LoadRevenue();
                txtStatus.Text = "✅ Cập nhật doanh thu hôm nay thành công!";
                txtStatus.Foreground = System.Windows.Media.Brushes.Green;
            }
            catch (Exception ex)
            {
                txtStatus.Text = "❌ Lỗi cập nhật: " + ex.Message;
                txtStatus.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
        // 🔹 Mở biểu đồ doanh thu theo tháng
        private void btnChart_Click(object sender, RoutedEventArgs e)
        {
            var chartWindow = new RevenueChartWindow();
            chartWindow.ShowDialog();
        }

    }
}
