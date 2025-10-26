using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BubbleTeaApp.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("UserID")]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        [Column("Username")]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        [Column("Password")]
        public string Password { get; set; } = string.Empty;
        
        [MaxLength(100)]
        [Column("FullName")]
        public string? FullName { get; set; }
        
        [Required]
        [MaxLength(20)]
        [Column("Role")]
        public string Role { get; set; } = string.Empty;
    }
}
