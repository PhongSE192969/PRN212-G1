using Microsoft.EntityFrameworkCore;
using MilkTea.DAL.Data;
using MilkTea.DAL.Models;

namespace MilkTea.BLL.Services
{
    public class RevenueService
    {
        public decimal GetRevenueByDate(DateTime date)
        {
            using var context = new TeaPOSDbContext();
            
            var revenue = context.Revenues.FirstOrDefault(r => r.ReportDate == date.Date);
            return revenue?.TotalRevenue ?? 0;
        }
        
        public List<Revenue> GetRevenueByMonth(int year, int month)
        {
            using var context = new TeaPOSDbContext();
            
            return context.Revenues
                .Where(r => r.ReportDate.Year == year && r.ReportDate.Month == month)
                .OrderBy(r => r.ReportDate)
                .ToList();
        }
        
        public List<ViewMonthlyRevenue> GetMonthlyRevenue()
        {
            using var context = new TeaPOSDbContext();
            
            return context.ViewMonthlyRevenues
                .FromSqlRaw("SELECT * FROM View_MonthlyRevenue")
                .ToList();
        }
        
        public List<ViewMonthlyRevenue> GetMonthlyRevenueByYear(int year)
        {
            using var context = new TeaPOSDbContext();
            
            return context.ViewMonthlyRevenues
                .FromSqlRaw("SELECT * FROM View_MonthlyRevenue WHERE Nam = {0}", year)
                .ToList();
        }
        
        public Dictionary<DateTime, decimal> GetRevenueByDateRange(DateTime fromDate, DateTime toDate)
        {
            using var context = new TeaPOSDbContext();
            
            return context.Revenues
                .Where(r => r.ReportDate >= fromDate.Date && r.ReportDate <= toDate.Date)
                .OrderBy(r => r.ReportDate)
                .ToDictionary(r => r.ReportDate, r => r.TotalRevenue);
        }
        
        public decimal GetTotalRevenue()
        {
            using var context = new TeaPOSDbContext();
            return context.Revenues.Sum(r => (decimal?)r.TotalRevenue) ?? 0;
        }
        
        public decimal GetTotalRevenueByYear(int year)
        {
            using var context = new TeaPOSDbContext();
            return context.Revenues
                .Where(r => r.ReportDate.Year == year)
                .Sum(r => (decimal?)r.TotalRevenue) ?? 0;
        }
        
        public decimal GetAverageRevenuePerDay(DateTime fromDate, DateTime toDate)
        {
            using var context = new TeaPOSDbContext();
            var revenues = context.Revenues
                .Where(r => r.ReportDate >= fromDate.Date && r.ReportDate <= toDate.Date)
                .ToList();
            
            return revenues.Any() ? revenues.Average(r => r.TotalRevenue) : 0;
        }
        
        // Top selling products
        public List<ProductRevenue> GetTopSellingProducts(int topCount = 10)
        {
            using var context = new TeaPOSDbContext();
            
            return context.InvoiceDetails
                .Include(d => d.Product)
                .GroupBy(d => new { d.ProductId, d.Product!.ProductName })
                .Select(g => new ProductRevenue
                {
                    ProductId = g.Key.ProductId ?? 0,
                    ProductName = g.Key.ProductName,
                    TotalQuantity = g.Sum(d => d.Quantity),
                    TotalRevenue = g.Sum(d => d.Subtotal)
                })
                .OrderByDescending(p => p.TotalRevenue)
                .Take(topCount)
                .ToList();
        }
        
        // Daily statistics
        public DailyStatistics GetDailyStatistics(DateTime date)
        {
            using var context = new TeaPOSDbContext();
            
            var invoices = context.Invoices
                .Where(i => i.InvoiceDate.Date == date.Date)
                .ToList();
            
            return new DailyStatistics
            {
                Date = date,
                TotalOrders = invoices.Count,
                TotalRevenue = invoices.Sum(i => i.FinalAmount),
                AverageOrderValue = invoices.Any() ? invoices.Average(i => i.FinalAmount) : 0,
                TotalDiscount = invoices.Sum(i => i.Discount),
                TotalVAT = invoices.Sum(i => i.VAT)
            };
        }
    }
    
    // DTOs for revenue reporting
    public class ProductRevenue
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }
    
    public class DailyStatistics
    {
        public DateTime Date { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalVAT { get; set; }
    }
}
