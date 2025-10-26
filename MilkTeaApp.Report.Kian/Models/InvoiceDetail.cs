using System;
using System.Collections.Generic;

namespace MilkTeaApp.Report.Kian.Models;

public partial class InvoiceDetail
{
    public int DetailId { get; set; }

    public int? InvoiceId { get; set; }

    public int? ProductId { get; set; }

    public int? ToppingId { get; set; }

    public int? Quantity { get; set; }

    public double? UnitPrice { get; set; }

    public double? Subtotal { get; set; }

    public virtual Invoice? Invoice { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Topping? Topping { get; set; }
}
