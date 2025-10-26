using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BubbleTeaApp.Models
{
    [Table("InvoiceDetails")]
    public class OrderDetail
    {
        [Key]
        [Column("DetailID")]
        public int Id { get; set; }
        
        [Column("InvoiceID")]
        public int? OrderId { get; set; }
        
        [Column("ProductID")]
        public int? ProductId { get; set; }
        
        [Column("ToppingID")]
        public int? ToppingId { get; set; }
        
        [Column("Quantity")]
        public int? Quantity { get; set; }
        
        [Column("UnitPrice", TypeName = "float")]
        public decimal? UnitPrice { get; set; }
        
        [Column("Subtotal", TypeName = "float")]
        public decimal? TotalPrice { get; set; }
        
        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }
        
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
        
        [ForeignKey("ToppingId")]
        public virtual Topping? Topping { get; set; }
        
        [NotMapped]
        public virtual ICollection<OrderTopping> OrderToppings { get; set; } = new List<OrderTopping>();
    }
}
