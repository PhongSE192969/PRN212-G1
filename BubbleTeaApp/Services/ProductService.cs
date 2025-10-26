using System.Collections.Generic;
using System.Linq;
using BubbleTeaApp.Data;
using BubbleTeaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BubbleTeaApp.Services
{
    public class ProductService
    {
        public List<Product> GetAllProducts()
        {
            using var context = new BubbleTeaDbContext();
            return context.Products
                .Include(p => p.CategoryNavigation)
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    ImageUrl = p.ImageUrl ?? "",
                    Category = p.CategoryNavigation != null ? p.CategoryNavigation.Name : "Chưa phân loại",
                    Description = p.CategoryNavigation != null ? p.CategoryNavigation.Name : ""
                })
                .ToList();
        }

        public Product? GetProductById(int id)
        {
            using var context = new BubbleTeaDbContext();
            var product = context.Products
                .Include(p => p.CategoryNavigation)
                .FirstOrDefault(p => p.Id == id);
            
            if (product != null && product.CategoryNavigation != null)
            {
                product.Category = product.CategoryNavigation.Name;
            }
            
            return product;
        }

        public List<Product> SearchProducts(string keyword)
        {
            using var context = new BubbleTeaDbContext();
            return context.Products
                .Include(p => p.CategoryNavigation)
                .Where(p => p.Name.Contains(keyword))
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    ImageUrl = p.ImageUrl ?? "",
                    Category = p.CategoryNavigation != null ? p.CategoryNavigation.Name : "Chưa phân loại",
                    Description = p.CategoryNavigation != null ? p.CategoryNavigation.Name : ""
                })
                .ToList();
        }

        public void AddProduct(Product product)
        {
            using var context = new BubbleTeaDbContext();
            context.Products.Add(product);
            context.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            using var context = new BubbleTeaDbContext();
            context.Products.Update(product);
            context.SaveChanges();
        }

        public void DeleteProduct(int id)
        {
            using var context = new BubbleTeaDbContext();
            var product = context.Products.Find(id);
            if (product != null)
            {
                context.Products.Remove(product);
                context.SaveChanges();
            }
        }
    }
}
