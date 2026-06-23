using Api.Contracts.Catalog;
using Api.Models.Entities.Shopping;

namespace Api.Contracts.Shopping;

public class CartItemResponse
{
  public Guid Id { get; init; }
  public Guid CartId { get; init; }
  public Guid ProductVariantId { get; init; }
  public int Quantity { get; init; }
  public ProductVariantResponse? ProductVariant { get; init; }

  public static CartItemResponse From(CartItem i) => new()
  {
    Id = i.Id,
    CartId = i.CartId,
    ProductVariantId = i.ProductVariantId,
    Quantity = i.Quantity,
    ProductVariant = i.ProductVariant != null ? ProductVariantResponse.From(i.ProductVariant) : null,
  };
}
