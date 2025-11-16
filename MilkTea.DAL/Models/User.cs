using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilkTea.DAL.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("UserID")]
        public int UserId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? FullName { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = string.Empty; // Admin, Staff
    }
}
