using System.Collections.Generic;
using System.Linq;
using BubbleTeaApp.Data;
using BubbleTeaApp.Models;

namespace BubbleTeaApp.Services
{
    public class ToppingService
    {
        public List<Topping> GetAllToppings()
        {
            using var context = new BubbleTeaDbContext();
            return context.Toppings.ToList();
        }

        public Topping? GetToppingById(int id)
        {
            using var context = new BubbleTeaDbContext();
            return context.Toppings.Find(id);
        }

        public void AddTopping(Topping topping)
        {
            using var context = new BubbleTeaDbContext();
            context.Toppings.Add(topping);
            context.SaveChanges();
        }

        public void UpdateTopping(Topping topping)
        {
            using var context = new BubbleTeaDbContext();
            context.Toppings.Update(topping);
            context.SaveChanges();
        }

        public void DeleteTopping(int id)
        {
            using var context = new BubbleTeaDbContext();
            var topping = context.Toppings.Find(id);
            if (topping != null)
            {
                context.Toppings.Remove(topping);
                context.SaveChanges();
            }
        }
    }
}
