using System.Windows;

namespace MilkTeaApp.Report.Kian
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); // ⚙️ Load toàn bộ UI từ XAML
        }

        // 🧾 Khi nhấn nút "Mở báo cáo doanh thu"
        private void btnOpenReport_Click(object sender, RoutedEventArgs e)
        {
            var report = new RevenueReportWindow(); // mở cửa sổ phụ
            report.ShowDialog();
        }
    }
}
