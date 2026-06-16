using Api.Models.Entities.Catalog;
using Api.Models.Entities.Identity;
using Api.Models.Entities.Orders;
using Api.Models.Entities.Payments;
using Api.Models.Entities.Reviews;
using Api.Models.Entities.Shopping;
using Api.Models.Entities.Wishlists;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api.Context;

public class DatabaseContext(
  DbContextOptions<DatabaseContext> options
) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{

  public DbSet<Category> Categories { get; set; }
  public DbSet<Product> Products { get; set; }
  public DbSet<ProductImage> ProductImages { get; set; }
  public DbSet<ProductVariant> ProductVariants { get; set; }
  public DbSet<SellerProfile> SellerProfiles { get; set; }
  public DbSet<Cart> Carts { get; set; }
  public DbSet<CartItem> CartItems { get; set; }
  public DbSet<Order> Orders { get; set; }
  public DbSet<OrderItem> OrderItems { get; set; }
  public DbSet<Address> Addresses { get; set; }
  public DbSet<Payment> Payments { get; set; }
  public DbSet<ProductReview> ProductReviews { get; set; }
  public DbSet<SellerReview> SellerReviews { get; set; }
  public DbSet<Wishlist> Wishlists { get; set; }
  public DbSet<WishlistItem> WishlistItems { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Cart → CartItems
    modelBuilder.Entity<CartItem>()
      .HasOne(ci => ci.Cart)
      .WithMany(c => c.CartItems)
      .HasForeignKey(ci => ci.CartId)
      .OnDelete(DeleteBehavior.Cascade);

    // Order → OrderItems
    modelBuilder.Entity<OrderItem>()
      .HasOne(oi => oi.Order)
      .WithMany(o => o.Items)
      .HasForeignKey(oi => oi.OrderId)
      .OnDelete(DeleteBehavior.Cascade);

    // Product → ProductImages
    modelBuilder.Entity<ProductImage>()
      .HasOne(pi => pi.Product)
      .WithMany(p => p.Images)
      .HasForeignKey(pi => pi.ProductId)
      .OnDelete(DeleteBehavior.Cascade);

    // Product → ProductVariants
    modelBuilder.Entity<ProductVariant>()
      .HasOne(pv => pv.Product)
      .WithMany(p => p.Variants)
      .HasForeignKey(pv => pv.ProductId)
      .OnDelete(DeleteBehavior.Cascade);

    // SellerProfile → Products
    modelBuilder.Entity<Product>()
      .HasOne(p => p.Seller)
      .WithMany(sp => sp.Products)
      .HasForeignKey(p => p.SellerId)
      .OnDelete(DeleteBehavior.Cascade);

    // Category (self-referencing)
    modelBuilder.Entity<Category>()
      .HasOne(c => c.ParentCategory)
      .WithMany()
      .HasForeignKey(c => c.ParentCategoryId)
      .OnDelete(DeleteBehavior.SetNull);

    // Wishlist → WishlistItems
    modelBuilder.Entity<WishlistItem>()
      .HasOne(wi => wi.Wishlist)
      .WithMany(w => w.Items)
      .HasForeignKey(wi => wi.WishlistId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}