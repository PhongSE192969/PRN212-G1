using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BubbleTeaApp.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [Column("ProductID")]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        [Column("ProductName")]
        public string Name { get; set; } = string.Empty;
        
        [Column("Price", TypeName = "float")]
        public decimal Price { get; set; }
        
        [Column("CategoryID")]
        public int? CategoryId { get; set; }
        
        [MaxLength(255)]
        [Column("ImagePath")]
        public string ImageUrl { get; set; } = string.Empty;
        
        [NotMapped]
        public string Description { get; set; } = string.Empty;
        
        [NotMapped]
        public string Category { get; set; } = string.Empty;
        
        // Navigation property
        [ForeignKey("CategoryId")]
        public virtual Category? CategoryNavigation { get; set; }
    }
}
