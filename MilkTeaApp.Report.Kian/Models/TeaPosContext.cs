using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MilkTeaApp.Report.Kian.Models;

public partial class TeaPosContext : DbContext
{
    public TeaPosContext()
    {
    }

    public TeaPosContext(DbContextOptions<TeaPosContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Revenue> Revenues { get; set; }

    public virtual DbSet<Topping> Toppings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<ViewMonthlyRevenue> ViewMonthlyRevenues { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Server=DESKTOP-AK87M8L\\SQLEXPRESS01;Database=TeaPOS;User Id=sa;Password=123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B86685232");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(50);
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__E43F6DF61B90DF63");

            entity.HasIndex(e => e.Code, "UQ__Discount__A25C5AA70286CFD1").IsUnique();

            entity.Property(e => e.DiscountId).HasColumnName("DiscountID");
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(100);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAD5603B4ADE");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_ApplyDiscount");
                    tb.HasTrigger("trg_UpdateRevenue");
                });

            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.Discount).HasDefaultValue(0.0);
            entity.Property(e => e.DiscountId).HasColumnName("DiscountID");
            entity.Property(e => e.FinalAmount).HasComputedColumnSql("(([TotalAmount]+[VAT])-[Discount])", true);
            entity.Property(e => e.InvoiceDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasDefaultValue("Tiền mặt");
            entity.Property(e => e.QrcodeData)
                .HasMaxLength(255)
                .HasColumnName("QRCodeData");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Đã thanh toán");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Vat)
                .HasDefaultValue(0.0)
                .HasColumnName("VAT");

            entity.HasOne(d => d.DiscountNavigation).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK__Invoices__Discou__619B8048");

            entity.HasOne(d => d.User).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Invoices__UserID__403A8C7D");
        });

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__InvoiceD__135C314DD11CBF58");

            entity.Property(e => e.DetailId).HasColumnName("DetailID");
            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Subtotal).HasComputedColumnSql("([Quantity]*[UnitPrice])", true);
            entity.Property(e => e.ToppingId).HasColumnName("ToppingID");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK__InvoiceDe__Invoi__46E78A0C");

            entity.HasOne(d => d.Product).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__InvoiceDe__Produ__47DBAE45");

            entity.HasOne(d => d.Topping).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.ToppingId)
                .HasConstraintName("FK__InvoiceDe__Toppi__48CFD27E");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6EDAF337A58");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ImagePath).HasMaxLength(255);
            entity.Property(e => e.ProductName).HasMaxLength(100);

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Products__Catego__5CD6CB2B");
        });

        modelBuilder.Entity<Revenue>(entity =>
        {
            entity.HasKey(e => e.RevenueId).HasName("PK__Revenue__275F173D122857F5");

            entity.ToTable("Revenue");

            entity.Property(e => e.RevenueId).HasColumnName("RevenueID");
            entity.Property(e => e.ReportDate).HasDefaultValueSql("(CONVERT([date],getdate()))");
            entity.Property(e => e.TotalRevenue).HasDefaultValue(0.0);
        });

        modelBuilder.Entity<Topping>(entity =>
        {
            entity.HasKey(e => e.ToppingId).HasName("PK__Toppings__EE02CCE5B406C420");

            entity.Property(e => e.ToppingId).HasColumnName("ToppingID");
            entity.Property(e => e.ToppingName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACC19E15C7");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4D7026FD2").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<ViewMonthlyRevenue>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_MonthlyRevenue");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
