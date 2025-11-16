using Microsoft.EntityFrameworkCore;
using MilkTea.DAL.Data;
using MilkTea.DAL.Models;

namespace MilkTea.BLL.Services
{
    public class ProductService
    {
        public List<Category> GetAllCategories()
        {
            using var context = new TeaPOSDbContext();
            return context.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.CategoryName)
                .ToList();
        }
        
        public List<Product> GetAllProducts()
        {
            using var context = new TeaPOSDbContext();
            return context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.ProductName)
                .ToList();
        }
        
        public List<Product> GetProductsByCategory(int categoryId)
        {
            using var context = new TeaPOSDbContext();
            return context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.ProductName)
                .ToList();
        }
        
        public Product? GetProductById(int productId)
        {
            using var context = new TeaPOSDbContext();
            return context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductId == productId);
        }
        
        public List<Product> SearchProducts(string keyword)
        {
            using var context = new TeaPOSDbContext();
            return context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductName.Contains(keyword))
                .OrderBy(p => p.ProductName)
                .ToList();
        }
        
        public List<Product> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            using var context = new TeaPOSDbContext();
            return context.Products
                .Include(p => p.Category)
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .OrderBy(p => p.Price)
                .ToList();
        }
        
        public int GetProductCount()
        {
            using var context = new TeaPOSDbContext();
            return context.Products.Count();
        }
        
        public int GetProductCountByCategory(int categoryId)
        {
            using var context = new TeaPOSDbContext();
            return context.Products.Count(p => p.CategoryId == categoryId);
        }

        public bool CreateProduct(Product product)
        {
            try
            {
                using var context = new TeaPOSDbContext();
                context.Products.Add(product);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateProduct(Product product)
        {
            try
            {
                using var context = new TeaPOSDbContext();
                context.Products.Update(product);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteProduct(int productId)
        {
            try
            {
                using var context = new TeaPOSDbContext();
                var product = context.Products.Find(productId);
                if (product != null)
                {
                    context.Products.Remove(product);
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
    }
}
