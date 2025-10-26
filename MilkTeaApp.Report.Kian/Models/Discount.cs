using System;
using System.Collections.Generic;

namespace MilkTeaApp.Report.Kian.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public double? Percentage { get; set; }

    public DateOnly? ExpireDate { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
