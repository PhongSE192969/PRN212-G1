namespace MilkTea.DAL.Models
{
    // DTO for Cart Item (not in DB)
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        
        // Topping info (optional)
        public int? ToppingId { get; set; }
        public string? ToppingName { get; set; }
        public decimal ToppingPrice { get; set; }
        
        // Calculated properties
        public decimal UnitPrice => ProductPrice + ToppingPrice;
        public decimal Subtotal => Quantity * UnitPrice;
        
        // Display
        public string DisplayName => string.IsNullOrEmpty(ToppingName) 
            ? ProductName 
            : $"{ProductName} + {ToppingName}";
    }
}
