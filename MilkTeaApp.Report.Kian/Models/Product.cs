using System;
using System.Collections.Generic;

namespace MilkTeaApp.Report.Kian.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Category { get; set; }

    public double Price { get; set; }

    public string? ImagePath { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? CategoryNavigation { get; set; }

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
}
