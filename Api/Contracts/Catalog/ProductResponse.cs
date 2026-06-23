using Api.Contracts.Identity;
using Api.Models.Entities.Catalog;

namespace Api.Contracts.Catalog;

public class ProductResponse
{
  public Guid Id { get; init; }
  public Guid SellerId { get; init; }
  public string? Name { get; init; }
  public string? Description { get; init; }
  public double BasePrice { get; init; }
  public double Discount { get; init; }
  public string? Material { get; init; }
  public ProductStatus? Status { get; init; }
  public DateTime CreatedAt { get; init; }
  public DateTime UpdatedAt { get; init; }
  public SellerProfileResponse? Seller { get; init; }
  public IEnumerable<CategoryResponse> Categories { get; init; } = [];
  public IEnumerable<ProductVariantResponse> Variants { get; init; } = [];
  public IEnumerable<ProductImageResponse> Images { get; init; } = [];

  public static ProductResponse From(Product p) => new()
  {
    Id = p.Id,
    SellerId = p.SellerId,
    Name = p.Name,
    Description = p.Description,
    BasePrice = p.BasePrice,
    Discount = p.Discount,
    Material = p.Material,
    Status = p.Status,
    CreatedAt = p.CreatedAt,
    UpdatedAt = p.UpdatedAt,
    Seller = p.Seller != null ? SellerProfileResponse.From(p.Seller) : null,
    Categories = p.Categories.Select(CategoryResponse.From),
    Variants = p.Variants.Select(ProductVariantResponse.From),
    Images = p.Images.Select(ProductImageResponse.From),
  };
}
