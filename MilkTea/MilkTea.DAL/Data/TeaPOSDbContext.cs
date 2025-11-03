using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MilkTea.DAL.Models;

namespace MilkTea.DAL.Data
{
    public class TeaPOSDbContext : DbContext
    {
        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Topping> Toppings { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<Revenue> Revenues { get; set; }
        public DbSet<ViewMonthlyRevenue> ViewMonthlyRevenues { get; set; }
        
        public TeaPOSDbContext()
        {
        }
        
        public TeaPOSDbContext(DbContextOptions<TeaPOSDbContext> options) : base(options)
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Đọc connection string từ appsettings.json
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
                
                var connectionString = configuration.GetConnectionString("TeaPOS");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure View as keyless
            modelBuilder.Entity<ViewMonthlyRevenue>().HasNoKey().ToView("View_MonthlyRevenue");
            
            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.DiscountNavigation)
                .WithMany(d => d.Invoices)
                .HasForeignKey(i => i.DiscountId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<InvoiceDetail>()
                .HasOne(id => id.Invoice)
                .WithMany(i => i.InvoiceDetails)
                .HasForeignKey(id => id.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<InvoiceDetail>()
                .HasOne(id => id.Product)
                .WithMany(p => p.InvoiceDetails)
                .HasForeignKey(id => id.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<InvoiceDetail>()
                .HasOne(id => id.Topping)
                .WithMany(t => t.InvoiceDetails)
                .HasForeignKey(id => id.ToppingId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Configure computed columns
            modelBuilder.Entity<Invoice>()
                .Property(i => i.FinalAmount)
                .HasComputedColumnSql("[TotalAmount] + [VAT] - [Discount]", stored: true);
            
            modelBuilder.Entity<InvoiceDetail>()
                .Property(id => id.Subtotal)
                .HasComputedColumnSql("[Quantity] * [UnitPrice]", stored: true);
        }
    }
}
