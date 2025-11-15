using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilkTea.DAL.Models
{
    [Table("Invoices")]
    public class Invoice
    {
        [Key]
        [Column("InvoiceID")]
        public int InvoiceId { get; set; }
        
        [Column(TypeName = "datetime")]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        
        [Column("UserID")]
        public int? UserId { get; set; }
        
        [Column(TypeName = "float")]
        public decimal TotalAmount { get; set; }
        
        [Column(TypeName = "float")]
        public decimal VAT { get; set; }
        
        [Column(TypeName = "float")]
        public decimal Discount { get; set; }
        
        // Computed column in DB, but we calculate it here too
        [Column(TypeName = "float")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal FinalAmount { get; set; }
        
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = "Tiền mặt";
        
        [MaxLength(255)]
        public string? QRCodeData { get; set; }
        
        [MaxLength(20)]
        public string Status { get; set; } = "Đã thanh toán";
        
        [Column("DiscountID")]
        public int? DiscountId { get; set; }
        
        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        
        [ForeignKey("DiscountId")]
        public virtual Discount? DiscountNavigation { get; set; }
        
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    }
}
