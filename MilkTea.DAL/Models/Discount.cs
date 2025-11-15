using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilkTea.DAL.Models
{
    [Table("Discounts")]
    public class Discount
    {
        [Key]
        [Column("DiscountID")]
        public int DiscountId { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? Description { get; set; }
        
        [Column(TypeName = "float")]
        public decimal Percentage { get; set; }
        
        [Column(TypeName = "date")]
        public DateTime? ExpireDate { get; set; }
        
        // Navigation property
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
