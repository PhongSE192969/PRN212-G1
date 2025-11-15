using Microsoft.EntityFrameworkCore;
using MilkTea.DAL.Data;
using MilkTea.DAL.Models;

namespace MilkTea.BLL.Services
{
    public class CategoryService
    {
        public List<Category> GetAllCategories()
        {
            using var context = new TeaPOSDbContext();
            return context.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.CategoryName)
                .ToList();
        }
        
        public Category? GetCategoryById(int categoryId)
        {
            using var context = new TeaPOSDbContext();
            return context.Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.CategoryId == categoryId);
        }
        
        public Category? GetCategoryByName(string categoryName)
        {
            using var context = new TeaPOSDbContext();
            return context.Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.CategoryName == categoryName);
        }
        
        public bool CreateCategory(Category category)
        {
            try
            {
                using var context = new TeaPOSDbContext();
                
                // Check if category name already exists
                if (context.Categories.Any(c => c.CategoryName == category.CategoryName))
                {
                    return false;
                }
                
                context.Categories.Add(category);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool UpdateCategory(Category category)
        {
            try
            {
                using var context = new TeaPOSDbContext();
                context.Categories.Update(category);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool DeleteCategory(int categoryId)
        {
            // Delete products in the category along with their invoice details to respect FK constraints
            using var context = new TeaPOSDbContext();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var productsInCategory = context.Products.Where(p => p.CategoryId == categoryId).ToList();
                if (productsInCategory.Any())
                {
                    var productIds = productsInCategory.Select(p => p.ProductId).ToList();
                    
                    // First, delete all InvoiceDetails that reference these products
                    var invoiceDetailsToDelete = context.InvoiceDetails
                        .Where(id => productIds.Contains(id.ProductId.Value))
                        .ToList();
                    context.InvoiceDetails.RemoveRange(invoiceDetailsToDelete);
                    context.SaveChanges();
                    
                    // Then delete the products
                    context.Products.RemoveRange(productsInCategory);
                    context.SaveChanges();
                }

                var category = context.Categories.Find(categoryId);
                if (category != null)
                {
                    context.Categories.Remove(category);
                    context.SaveChanges();
                    transaction.Commit();
                    return true;
                }

                transaction.Rollback();
                return false;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        
        public int GetCategoryCount()
        {
            using var context = new TeaPOSDbContext();
            return context.Categories.Count();
        }
        
        public List<CategoryStatistics> GetCategoriesWithStatistics()
        {
            using var context = new TeaPOSDbContext();
            
            return context.Categories
                .Select(c => new CategoryStatistics
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    ProductCount = c.Products.Count,
                    TotalRevenue = c.Products
                        .SelectMany(p => p.InvoiceDetails)
                        .Sum(d => (decimal?)d.Subtotal) ?? 0
                })
                .OrderByDescending(c => c.TotalRevenue)
                .ToList();
        }
    }
    
    public class CategoryStatistics
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
