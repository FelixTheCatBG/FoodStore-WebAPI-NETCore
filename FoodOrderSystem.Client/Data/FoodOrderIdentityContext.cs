using FoodOrderSystem.Client.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrderSystem.Client.Data
{
    public class FoodOrderIdentityContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<ProductIngredient> ProductIngredients { get; set; }
        

        public FoodOrderIdentityContext() { }
        public FoodOrderIdentityContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Creating roles
            builder.Entity<IdentityRole>().HasData(
                new { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new { Id = "2", Name = "Customer", NormalizedName = "CUSTOMER" }
            );

            // User - Orders One-to-Many
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.ApplicationUser)
                .HasForeignKey(o => o.ApplicationUserId);

            // Category - Product One-to-Many
            builder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId);

            // Order - Product Many-to-Many Composite Key
            builder.Entity<OrderProduct>()
               .HasKey(po => new { po.ProductId, po.OrderId });

            builder.Entity<Product>()
               .HasMany(p => p.OrderProducts)
               .WithOne(cd => cd.Product)
               .HasForeignKey(cd => cd.ProductId);

            builder.Entity<Order>()
               .HasMany(o => o.OrderProducts)
               .WithOne(po => po.Order)
               .HasForeignKey(po => po.OrderId);

            // Product - Ingredient Relation Composite Key
            builder.Entity<ProductIngredient>()
               .HasKey(po => new { po.ProductId, po.IngredientId });

            builder.Entity<Product>()
               .HasMany(p => p.ProductIngredients)
               .WithOne(cd => cd.Product)
               .HasForeignKey(cd => cd.ProductId);

            builder.Entity<Ingredient>()
                .HasMany(o => o.ProductIngredients)
                .WithOne(po => po.Ingredient)
                .HasForeignKey(po => po.IngredientId);

            // Product - Discount One-to-Zero
            builder.Entity<Product>()
                .HasOne(p => p.Discount)
                .WithOne(d => d.Product)
                .HasForeignKey<Discount>(d => d.ProductId);

            //Decimal options for Db Creation
            builder.Entity<Product>()
                 .Property(p => p.Price)
                 .HasColumnType("decimal(18,2)");

            builder.Entity<Discount>()
               .Property(p => p.DiscountedPrice)
               .HasColumnType("decimal(18,2)");

            builder.Entity<OrderProduct>()
              .Property(p => p.NetPrice)
              .HasColumnType("decimal(18,2)");

            builder.Entity<Order>()
              .Property(p => p.TotalPrice)
              .HasColumnType("decimal(18,2)");

            //Ignore Calculated Property from Db
            builder.Entity<OrderProduct>()
                .Ignore(op => op.CalculatedPrice);
        }
    }
}
