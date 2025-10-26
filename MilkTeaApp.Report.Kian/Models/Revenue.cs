using System;
using System.Collections.Generic;

namespace MilkTeaApp.Report.Kian.Models;

public partial class Revenue
{
    public int RevenueId { get; set; }

    public DateOnly? ReportDate { get; set; }

    public double? TotalRevenue { get; set; }
}
