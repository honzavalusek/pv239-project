using MalyFarmar.Api.DAL.Enums;
using MalyFarmar.Api.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MalyFarmar.Api.DAL.Data;

public class MalyFarmarDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }

    public MalyFarmarDbContext(DbContextOptions<MalyFarmarDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Product 0..*  -- 1 Seller
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Seller)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.SellerId);

        // Order 1..* -- 1 Buyer
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Buyer)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.BuyerId);

        // Order 1..* -- 1 OrderStatus
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Status)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.StatusId);

        // OrderItem 1..* -- 1 Order
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId);

        // OrderItem 1..* -- 1 Product
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId);

        // Seed order statuses
        modelBuilder.Entity<OrderStatus>()
            .HasData(new List<OrderStatus>
            {
                new OrderStatus { Id = OrderStatusEnum.Created, Name = "Created" },
                new OrderStatus { Id = OrderStatusEnum.PickUpSet, Name = "Pick-Up Set" },
                new OrderStatus { Id = OrderStatusEnum.Completed, Name = "Completed" }
            });

        base.OnModelCreating(modelBuilder);
    }
}