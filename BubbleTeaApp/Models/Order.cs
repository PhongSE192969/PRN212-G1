using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BubbleTeaApp.Models
{
    [Table("Invoices")]
    public class Order
    {
        [Key]
        [Column("InvoiceID")]
        public int Id { get; set; }
        
        [Column("InvoiceDate")]
        public DateTime? OrderDate { get; set; }
        
        [Column("UserID")]
        public int? UserId { get; set; }
        
        [Column("TotalAmount", TypeName = "float")]
        public decimal TotalAmount { get; set; }
        
        [Column("VAT", TypeName = "float")]
        public decimal? VAT { get; set; }
        
        [Column("Discount", TypeName = "float")]
        public decimal? Discount { get; set; }
        
        [Column("FinalAmount", TypeName = "float")]
        public decimal? FinalAmount { get; set; }
        
        [MaxLength(50)]
        [Column("PaymentMethod")]
        public string PaymentMethod { get; set; } = string.Empty;
        
        [MaxLength(255)]
        [Column("QRCodeData")]
        public string QRCodeData { get; set; } = string.Empty;
        
        [MaxLength(20)]
        [Column("Status")]
        public string Status { get; set; } = "Pending";
        
        [Column("DiscountID")]
        public int? DiscountID { get; set; }
        
        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
