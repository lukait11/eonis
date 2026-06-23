using Api.Models.Entities.Catalog;

namespace Api.Contracts.Catalog;

public class ProductVariantResponse
{
  public Guid Id { get; init; }
  public Guid ProductId { get; init; }
  public string? Size { get; init; }
  public string? Color { get; init; }
  public int Quantity { get; init; }

  public static ProductVariantResponse From(ProductVariant v) => new()
  {
    Id = v.Id,
    ProductId = v.ProductId,
    Size = v.Size,
    Color = v.Color,
    Quantity = v.Quantity,
  };
}
