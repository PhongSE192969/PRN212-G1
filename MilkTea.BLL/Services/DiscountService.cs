using MilkTea.DAL.Data;
using MilkTea.DAL.Models;

namespace MilkTea.BLL.Services
{
    public class DiscountService
    {
        public Discount? GetValidDiscountByCode(string code)
        {
            using var context = new TeaPOSDbContext();
            
            var discount = context.Discounts
                .FirstOrDefault(d => d.Code.ToUpper() == code.ToUpper() 
                    && (d.ExpireDate == null || d.ExpireDate >= DateTime.Today));
            
            return discount;
        }
        
        public List<Discount> GetAllValidDiscounts()
        {
            using var context = new TeaPOSDbContext();
            
            return context.Discounts
                .Where(d => d.ExpireDate == null || d.ExpireDate >= DateTime.Today)
                .OrderBy(d => d.Code)
                .ToList();
        }
        
        public List<Discount> GetAllDiscounts()
        {
            using var context = new TeaPOSDbContext();
            return context.Discounts.OrderBy(d => d.Code).ToList();
        }
        
        public Discount? GetDiscountById(int discountId)
        {
            using var context = new TeaPOSDbContext();
            return context.Discounts.Find(discountId);
        }
        
        public bool CreateDiscount(Discount discount)
        {
            try
            {
                using var context = new TeaPOSDbContext();
                
                // Check if code already exists
                if (context.Discounts.Any(d => d.Code.ToUpper() == discount.Code.ToUpper()))
                {
                    throw new Exception($"Mã giảm giá '{discount.Code}' đã tồn tại!");
                }
                
                context.Discounts.Add(discount);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool UpdateDiscount(Discount discount)
        {
            try
            {
                using var context = new TeaPOSDbContext();
                
                // Check if code already exists for another discount
                if (context.Discounts.Any(d => d.DiscountId != discount.DiscountId && d.Code.ToUpper() == discount.Code.ToUpper()))
                {
                    throw new Exception($"Mã giảm giá '{discount.Code}' đã tồn tại!");
                }
                
                context.Discounts.Update(discount);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool DeleteDiscount(int discountId)
        {
            try
            {
                using var context = new TeaPOSDbContext();
                var discount = context.Discounts.Find(discountId);
                if (discount != null)
                {
                    context.Discounts.Remove(discount);
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
        
        public List<Discount> GetExpiredDiscounts()
        {
            using var context = new TeaPOSDbContext();
            return context.Discounts
                .Where(d => d.ExpireDate != null && d.ExpireDate < DateTime.Today)
                .OrderByDescending(d => d.ExpireDate)
                .ToList();
        }
        
        public int GetDiscountCount()
        {
            using var context = new TeaPOSDbContext();
            return context.Discounts.Count();
        }
        
        public int GetValidDiscountCount()
        {
            using var context = new TeaPOSDbContext();
            return context.Discounts.Count(d => d.ExpireDate == null || d.ExpireDate >= DateTime.Today);
        }
        
        public decimal CalculateDiscountAmount(int discountId, decimal totalAmount)
        {
            var discount = GetDiscountById(discountId);
            if (discount != null && (discount.ExpireDate == null || discount.ExpireDate >= DateTime.Today))
            {
                return totalAmount * (discount.Percentage / 100);
            }
            return 0;
        }
    }
}
