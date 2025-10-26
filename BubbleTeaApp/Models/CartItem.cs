using System.Collections.Generic;
using System.Linq;

namespace BubbleTeaApp.Models
{
    public class CartItem
    {
        public Product Product { get; set; } = new Product();
        public int Quantity { get; set; }
        public List<Topping> SelectedToppings { get; set; } = new List<Topping>();
        
        public decimal TotalPrice
        {
            get
            {
                decimal productTotal = Product.Price * Quantity;
                decimal toppingsTotal = SelectedToppings.Sum(t => t.Price) * Quantity;
                return productTotal + toppingsTotal;
            }
        }

        public string ToppingsDisplay
        {
            get
            {
                if (SelectedToppings.Count == 0)
                    return "Không topping";
                return string.Join(", ", SelectedToppings.Select(t => t.Name));
            }
        }
    }
}
