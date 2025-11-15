using System.Windows;
using MilkTea.GUI.ViewModels;

namespace MilkTea.GUI.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Username = txtUsername.Text;
            _viewModel.Password = txtPassword.Password;

            if (_viewModel.Login())
            {
                // Open main window
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                txtError.Text = _viewModel.ErrorMessage;
            }
        }
    }
}
