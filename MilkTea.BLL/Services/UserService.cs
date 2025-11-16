using MilkTea.DAL.Data;
using MilkTea.DAL.Models;

namespace MilkTea.BLL.Services
{
    public class UserService
    {
        public List<User> GetAllUsers()
        {
            using var context = new TeaPOSDbContext();
            return context.Users.OrderBy(u => u.Username).ToList();
        }
        
        public User? GetUserById(int userId)
        {
            using var context = new TeaPOSDbContext();
            return context.Users.Find(userId);
        }
        
        public User? GetUserByUsername(string username)
        {
            using var context = new TeaPOSDbContext();
            return context.Users.FirstOrDefault(u => u.Username == username);
        }
        
        public bool CreateUser(User user)
        {
            try
            {
                using var context = new TeaPOSDbContext();
                
                // Check if username already exists
                if (context.Users.Any(u => u.Username == user.Username))
                {
                    return false;
                }
                
                context.Users.Add(user);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool UpdateUser(User user)
        {
            try
            {
                using var context = new TeaPOSDbContext();
                context.Users.Update(user);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool DeleteUser(int userId)
        {
            try
            {
                using var context = new TeaPOSDbContext();
                var user = context.Users.Find(userId);
                if (user != null)
                {
                    context.Users.Remove(user);
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        
        public bool ValidateUser(string username, string password)
        {
            using var context = new TeaPOSDbContext();
            return context.Users.Any(u => u.Username == username && u.Password == password);
        }
        
        public List<User> GetUsersByRole(string role)
        {
            using var context = new TeaPOSDbContext();
            return context.Users
                .Where(u => u.Role == role)
                .OrderBy(u => u.Username)
                .ToList();
        }
        
        public int GetUserCount()
        {
            using var context = new TeaPOSDbContext();
            return context.Users.Count();
        }
        
        public int GetUserCountByRole(string role)
        {
            using var context = new TeaPOSDbContext();
            return context.Users.Count(u => u.Role == role);
        }
    }
}
