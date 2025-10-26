using System;
using System.Collections.Generic;

namespace MilkTeaApp.Report.Kian.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public DateTime? InvoiceDate { get; set; }

    public int? UserId { get; set; }

    public double TotalAmount { get; set; }

    public double? Vat { get; set; }

    public double? Discount { get; set; }

    public double? FinalAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? QrcodeData { get; set; }

    public string? Status { get; set; }

    public int? DiscountId { get; set; }

    public virtual Discount? DiscountNavigation { get; set; }

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    public virtual User? User { get; set; }
}
