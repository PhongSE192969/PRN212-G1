using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilkTea.DAL.Models
{
    [Table("Revenue")]
    public class Revenue
    {
        [Key]
        [Column("RevenueID")]
        public int RevenueId { get; set; }
        
        [Column(TypeName = "date")]
        public DateTime ReportDate { get; set; } = DateTime.Today;
        
        [Column(TypeName = "float")]
        public decimal TotalRevenue { get; set; }
    }
}
