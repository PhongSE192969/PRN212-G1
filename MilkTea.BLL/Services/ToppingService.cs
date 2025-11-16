using MilkTea.DAL.Data;
using MilkTea.DAL.Models;

namespace MilkTea.BLL.Services
{
    public class ToppingService
    {
        public List<Topping> GetAllToppings()
        {
            using var context = new TeaPOSDbContext();
            return context.Toppings
                .OrderBy(t => t.ToppingName)
                .ToList();
        }
        
        public Topping? GetToppingById(int toppingId)
        {
            using var context = new TeaPOSDbContext();
            return context.Toppings.Find(toppingId);
        }
        
        public Topping? GetToppingByName(string toppingName)
        {
            using var context = new TeaPOSDbContext();
            return context.Toppings.FirstOrDefault(t => t.ToppingName == toppingName);
        }
        
        public List<Topping> SearchToppings(string keyword)
        {
            using var context = new TeaPOSDbContext();
            return context.Toppings
                .Where(t => t.ToppingName!.Contains(keyword))
                .OrderBy(t => t.ToppingName)
                .ToList();
        }
        
        public bool CreateTopping(Topping topping)
        {
            try
            {
                using var context = new TeaPOSDbContext();
                context.Toppings.Add(topping);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool UpdateTopping(Topping topping)
        {
            try
            {
                using var context = new TeaPOSDbContext();
                context.Toppings.Update(topping);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool DeleteTopping(int toppingId)
        {
            try
            {
                using var context = new TeaPOSDbContext();
                var topping = context.Toppings.Find(toppingId);
                if (topping != null)
                {
                    context.Toppings.Remove(topping);
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
        
        public int GetToppingCount()
        {
            using var context = new TeaPOSDbContext();
            return context.Toppings.Count();
        }
        
        public decimal GetAverageToppingPrice()
        {
            using var context = new TeaPOSDbContext();
            return context.Toppings.Average(t => (decimal?)t.Price) ?? 0;
        }
    }
}
