using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BubbleTeaApp.Models
{
    public class OrderTopping
    {
        [Key]
        public int Id { get; set; }
        
        public int OrderDetailId { get; set; }
        public int ToppingId { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        // Navigation properties
        public virtual OrderDetail OrderDetail { get; set; } = null!;
        public virtual Topping Topping { get; set; } = null!;
    }
}
