using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BubbleTeaApp.Models
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        [Column("CategoryID")]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        [Column("CategoryName")]
        public string Name { get; set; } = string.Empty;
    }
}
