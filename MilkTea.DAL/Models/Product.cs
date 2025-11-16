using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilkTea.DAL.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [Column("ProductID")]
        public int ProductId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; } = string.Empty;
        
        [Column("CategoryID")]
        public int? CategoryId { get; set; }
        
        [Column(TypeName = "float")]
        public decimal Price { get; set; }
        
        [MaxLength(255)]
        public string? ImagePath { get; set; }
        
        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
        
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
        
        // Display properties (not mapped to DB)
        [NotMapped]
        public string CategoryName => Category?.CategoryName ?? "Chưa phân loại";
        
        [NotMapped]
        public string DisplayPrice => $"{Price:N0} đ";
    }
}
