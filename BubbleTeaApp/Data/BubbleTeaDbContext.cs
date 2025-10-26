using Microsoft.EntityFrameworkCore;
using BubbleTeaApp.Models;

namespace BubbleTeaApp.Data
{
    public class BubbleTeaDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Topping> Toppings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Kết nối đến database TeaPOS của bạn
            optionsBuilder.UseSqlServer(
                @"Server=ASUS\MSSQL2022;Database=TeaPOS;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình các quan hệ
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Topping)
                .WithMany()
                .HasForeignKey(od => od.ToppingId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.CategoryNavigation)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
