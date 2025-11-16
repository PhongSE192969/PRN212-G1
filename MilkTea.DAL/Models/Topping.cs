using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilkTea.DAL.Models
{
    [Table("Toppings")]
    public class Topping
    {
        [Key]
        [Column("ToppingID")]
        public int ToppingId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string ToppingName { get; set; } = string.Empty;
        
        [Column(TypeName = "float")]
        public decimal Price { get; set; }
        
        // Navigation property
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    }
}
