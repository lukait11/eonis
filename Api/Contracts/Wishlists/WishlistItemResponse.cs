using Api.Contracts.Catalog;
using Api.Models.Entities.Wishlists;

namespace Api.Contracts.Wishlists;

public class WishlistItemResponse
{
  public Guid Id { get; init; }
  public Guid WishlistId { get; init; }
  public Guid ProductId { get; init; }
  public DateTime AddedAt { get; init; }
  public ProductResponse? Product { get; init; }

  public static WishlistItemResponse From(WishlistItem i) => new()
  {
    Id = i.Id,
    WishlistId = i.WishlistId,
    ProductId = i.ProductId,
    AddedAt = i.AddedAt,
    Product = i.Product != null ? ProductResponse.From(i.Product) : null,
  };
}
