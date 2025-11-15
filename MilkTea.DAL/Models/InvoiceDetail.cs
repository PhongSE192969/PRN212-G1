using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilkTea.DAL.Models
{
    [Table("InvoiceDetails")]
    public class InvoiceDetail
    {
        [Key]
        [Column("DetailID")]
        public int DetailId { get; set; }
        
        [Column("InvoiceID")]
        public int? InvoiceId { get; set; }
        
        [Column("ProductID")]
        public int? ProductId { get; set; }
        
        [Column("ToppingID")]
        public int? ToppingId { get; set; }
        
        public int Quantity { get; set; }
        
        [Column(TypeName = "float")]
        public decimal UnitPrice { get; set; }
        
        // Computed column in DB
        [Column(TypeName = "float")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal Subtotal { get; set; }
        
        // Navigation properties
        [ForeignKey("InvoiceId")]
        public virtual Invoice? Invoice { get; set; }
        
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
        
        [ForeignKey("ToppingId")]
        public virtual Topping? Topping { get; set; }
    }
}
