using Api.Models.Entities.Catalog;

namespace Api.Contracts.Catalog;

public class ProductImageResponse
{
  public Guid Id { get; init; }
  public Guid ProductId { get; init; }
  public string? ImageUrl { get; init; }
  public bool IsPrimary { get; init; }

  public static ProductImageResponse From(ProductImage i) => new()
  {
    Id = i.Id,
    ProductId = i.ProductId,
    ImageUrl = i.ImageUrl,
    IsPrimary = i.IsPrimary,
  };
}
