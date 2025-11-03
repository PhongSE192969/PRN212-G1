using System.ComponentModel.DataAnnotations.Schema;

namespace MilkTea.DAL.Models
{
    // View in database
    [Table("View_MonthlyRevenue")]
    public class ViewMonthlyRevenue
    {
        [Column("Nam")]
        public int? Year { get; set; }
        
        [Column("Thang")]
        public int? Month { get; set; }
        
        [Column("TongDoanhThu")]
        public double? TotalRevenue { get; set; }
    }
}
