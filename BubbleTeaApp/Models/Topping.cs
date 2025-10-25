using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BubbleTeaApp.Models
{
    [Table("Toppings")]
    public class Topping
    {
        [Key]
        [Column("ToppingID")]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        [Column("ToppingName")]
        public string Name { get; set; } = string.Empty;
        
        [Column("Price", TypeName = "float")]
        public decimal Price { get; set; }
    }
}
