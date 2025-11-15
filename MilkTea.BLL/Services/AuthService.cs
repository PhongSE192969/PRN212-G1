using MilkTea.DAL.Data;
using MilkTea.DAL.Models;

namespace MilkTea.BLL.Services
{
    public class AuthService
    {
        public User? Login(string username, string password)
        {
            using var context = new TeaPOSDbContext();
            
            var user = context.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);
            
            return user;
        }
        
        public bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            using var context = new TeaPOSDbContext();
            
            var user = context.Users.Find(userId);
            if (user == null || user.Password != oldPassword)
                return false;
            
            user.Password = newPassword;
            context.SaveChanges();
            return true;
        }
    }
}
