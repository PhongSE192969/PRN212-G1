using System.Windows;
using MilkTea.DAL.Models;
using MilkTea.BLL.Services;
using MilkTea.GUI.Utils;

namespace MilkTea.GUI.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        
        public LoginViewModel()
        {
            _authService = new AuthService();
        }
        
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }
        
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
        
        public bool Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ thông tin!";
                return false;
            }
            
            var user = _authService.Login(Username, Password);
            
            if (user == null)
            {
                ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return false;
            }
            
            // Save current user to global config
            AppConfig.CurrentUser = user;
            ErrorMessage = string.Empty;
            return true;
        }
    }
}
