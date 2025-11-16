using Microsoft.EntityFrameworkCore;
using MilkTea.DAL.Data;
using MilkTea.DAL.Models;

namespace MilkTea.BLL.Services
{
    public class OrderService
    {
        // For backward compatibility with old modules
        public class Order
        {
            public int OrderId { get; set; }
            public DateTime OrderDate { get; set; }
            public int? UserId { get; set; }
            public decimal TotalAmount { get; set; }
            public string Status { get; set; } = "Pending";
        }
        
        public class OrderDetail
        {
            public int DetailId { get; set; }
            public int OrderId { get; set; }
            public int ProductId { get; set; }
            public int? ToppingId { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }
        
        public List<Invoice> GetAllOrders()
        {
            using var context = new TeaPOSDbContext();
            return context.Invoices
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();
        }
        
        public Invoice? GetOrderById(int orderId)
        {
            using var context = new TeaPOSDbContext();
            return context.Invoices
                .Include(i => i.User)
                .Include(i => i.DiscountNavigation)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Product)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Topping)
                .FirstOrDefault(i => i.InvoiceId == orderId);
        }
        
        public List<Invoice> GetOrdersByUser(int userId)
        {
            using var context = new TeaPOSDbContext();
            return context.Invoices
                .Include(i => i.InvoiceDetails)
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();
        }
        
        public List<Invoice> GetOrdersByDate(DateTime date)
        {
            using var context = new TeaPOSDbContext();
            return context.Invoices
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails)
                .Where(i => i.InvoiceDate.Date == date.Date)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();
        }
        
        public List<Invoice> GetOrdersByDateRange(DateTime fromDate, DateTime toDate)
        {
            using var context = new TeaPOSDbContext();
            return context.Invoices
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails)
                .Where(i => i.InvoiceDate.Date >= fromDate.Date && i.InvoiceDate.Date <= toDate.Date)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();
        }
        
        public List<Invoice> GetOrdersByStatus(string status)
        {
            using var context = new TeaPOSDbContext();
            return context.Invoices
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails)
                .Where(i => i.Status == status)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();
        }
        
        public int GetOrderCount()
        {
            using var context = new TeaPOSDbContext();
            return context.Invoices.Count();
        }
        
        public int GetOrderCountByDate(DateTime date)
        {
            using var context = new TeaPOSDbContext();
            return context.Invoices.Count(i => i.InvoiceDate.Date == date.Date);
        }
        
        public decimal GetTotalRevenueByDate(DateTime date)
        {
            using var context = new TeaPOSDbContext();
            return context.Invoices
                .Where(i => i.InvoiceDate.Date == date.Date)
                .Sum(i => (decimal?)i.FinalAmount) ?? 0;
        }
        
        public decimal GetTotalRevenueByDateRange(DateTime fromDate, DateTime toDate)
        {
            using var context = new TeaPOSDbContext();
            return context.Invoices
                .Where(i => i.InvoiceDate.Date >= fromDate.Date && i.InvoiceDate.Date <= toDate.Date)
                .Sum(i => (decimal?)i.FinalAmount) ?? 0;
        }
    }
}
