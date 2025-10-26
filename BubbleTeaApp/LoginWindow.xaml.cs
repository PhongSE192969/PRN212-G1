using BubbleTeaApp.Data;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BubbleTeaApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            var placeholder = passwordBox.Template.FindName("Placeholder", passwordBox) as TextBlock;
            if (placeholder != null)
            {
                placeholder.Visibility = string.IsNullOrEmpty(passwordBox.Password)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new BubbleTeaDbContext())
                {
                    var user = db.Users
                                 .FirstOrDefault(u => u.Username == username && u.Password == password);

                    if (user != null)
                    {
                        MessageBox.Show($"Xin chào {user.FullName ?? user.Username}!", "Đăng nhập thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        MainWindow main = new MainWindow();
                        main.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối CSDL:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
