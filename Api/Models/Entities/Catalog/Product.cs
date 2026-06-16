using Api.Models.Entities.Identity;

namespace Api.Models.Entities.Catalog;

public class Product
{
  public Guid Id { get; set; }
  public Guid SellerId { get; set; }
  public Guid CategoryId { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public double BasePrice { get; set; }
  public double Discount { get; set; }
  public string? Material { get; set; }
  public string? Status { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; }

  // Navigation properties
  public SellerProfile? Seller { get; set; }
  public Category? Category { get; set; }
  public ICollection<ProductVariant> Variants { get; set; } = [];
  public ICollection<ProductImage> Images { get; set; } = [];
}