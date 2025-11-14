using MilkTea.BLL;
using Microsoft.EntityFrameworkCore;
using MilkTea.DAL.Data;
using MilkTea.DAL.Models;
// using MilkTea.GUI.Utils; // Will be added later

namespace MilkTea.BLL.Services
{
    public class InvoiceService
    {
        public int CreateInvoice(List<CartItem> cartItems, int? discountId, string paymentMethod, string? qrCodeData = null)
        {
            if (cartItems == null || cartItems.Count == 0)
            {
                throw new ArgumentException("Giỏ hàng không được rỗng!");
            }
            
            using var context = new TeaPOSDbContext();
            using var transaction = context.Database.BeginTransaction();
            
            try
            {
                // Calculate totals
                decimal totalAmount = cartItems.Sum(item => item.Subtotal);
                decimal vat = totalAmount * AppConfig.VATRate;
                
                // Get discount
                decimal discountAmount = 0;
                if (discountId.HasValue)
                {
                    var discount = context.Discounts.Find(discountId.Value);
                    if (discount != null && (discount.ExpireDate == null || discount.ExpireDate >= DateTime.Today))
                    {
                        discountAmount = totalAmount * (discount.Percentage / 100);
                    }
                }
                
                // Create invoice
                var invoice = new Invoice
                {
                    InvoiceDate = DateTime.Now,
                    UserId = AppConfig.CurrentUser?.UserId,
                    TotalAmount = totalAmount,
                    VAT = vat,
                    Discount = discountAmount,
                    PaymentMethod = paymentMethod,
                    QRCodeData = qrCodeData,
                    Status = "Đã thanh toán",
                    DiscountId = discountId
                };
                
                // Use raw SQL to insert invoice (bypass EF Core tracking for trigger compatibility)
                var sql = @"
                    INSERT INTO Invoices (InvoiceDate, UserID, TotalAmount, VAT, Discount, PaymentMethod, QRCodeData, Status, DiscountID)
                    VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8});
                    SELECT CAST(SCOPE_IDENTITY() as int);";
                
                var invoiceId = context.Database.SqlQueryRaw<int>(
                    sql,
                    invoice.InvoiceDate,
                    invoice.UserId.HasValue ? (object)invoice.UserId.Value : DBNull.Value,
                    invoice.TotalAmount,
                    invoice.VAT,
                    invoice.Discount,
                    invoice.PaymentMethod,
                    invoice.QRCodeData ?? (object)DBNull.Value,
                    invoice.Status,
                    invoice.DiscountId.HasValue ? (object)invoice.DiscountId.Value : DBNull.Value
                ).AsEnumerable().FirstOrDefault();
                
                if (invoiceId == 0)
                {
                    throw new Exception("Không thể tạo hóa đơn!");
                }
                
                invoice.InvoiceId = invoiceId;

                // Create invoice details
                foreach (var item in cartItems)
                {
                    var detail = new InvoiceDetail
                    {
                        InvoiceId = invoice.InvoiceId,

                        // FIX: nếu ProductId = 0 (topping mua riêng) thì cho DB là NULL
                        ProductId = (item.ProductId == 0) ? null : item.ProductId,

                        // OPTIONAL: nếu bạn có dùng 0 cho "không có topping" thì làm giống vậy,
                        // còn nếu luôn null rồi thì giữ nguyên cũng được
                        ToppingId = (item.ToppingId == 0) ? null : item.ToppingId,

                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                        // Subtotal là computed column trong DB nên không cần set
                    };

                    context.InvoiceDetails.Add(detail);
                }

                context.SaveChanges();
                transaction.Commit();
                
                return invoice.InvoiceId;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Lỗi khi tạo hóa đơn: {ex.Message}", ex);
            }
        }
        
        public Invoice? GetInvoiceById(int invoiceId)
        {
            using var context = new TeaPOSDbContext();
            
            return context.Invoices
                .Include(i => i.User)
                .Include(i => i.DiscountNavigation)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Product)
                        .ThenInclude(p => p!.Category)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Topping)
                .FirstOrDefault(i => i.InvoiceId == invoiceId);
        }
        
        public List<Invoice> GetInvoicesByDate(DateTime date)
        {
            using var context = new TeaPOSDbContext();
            
            return context.Invoices
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails)
                .Where(i => i.InvoiceDate.Date == date.Date)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();
        }
        
        public List<Invoice> GetInvoicesByDateRange(DateTime fromDate, DateTime toDate)
        {
            using var context = new TeaPOSDbContext();
            
            return context.Invoices
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails)
                .Where(i => i.InvoiceDate.Date >= fromDate.Date && i.InvoiceDate.Date <= toDate.Date)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();
        }
        
        public List<Invoice> GetInvoicesByUser(int userId)
        {
            using var context = new TeaPOSDbContext();
            
            return context.Invoices
                .Include(i => i.InvoiceDetails)
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();
        }
        
        public List<Invoice> GetRecentInvoices(int count = 10)
        {
            using var context = new TeaPOSDbContext();
            
            return context.Invoices
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails)
                .OrderByDescending(i => i.InvoiceDate)
                .Take(count)
                .ToList();
        }
        
        public bool UpdateInvoiceStatus(int invoiceId, string newStatus)
        {
            try
            {
                using var context = new TeaPOSDbContext();
                var invoice = context.Invoices.Find(invoiceId);
                if (invoice != null)
                {
                    invoice.Status = newStatus;
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        
        public bool CancelInvoice(int invoiceId)
        {
            return UpdateInvoiceStatus(invoiceId, "Đã hủy");
        }
        
        public int GetInvoiceCount()
        {
            using var context = new TeaPOSDbContext();
            return context.Invoices.Count();
        }
        
        public int GetInvoiceCountByDate(DateTime date)
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
    }
}
