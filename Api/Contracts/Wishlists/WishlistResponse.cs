using Api.Models.Entities.Wishlists;

namespace Api.Contracts.Wishlists;

public class WishlistResponse
{
  public Guid Id { get; init; }
  public Guid UserId { get; init; }
  public IEnumerable<WishlistItemResponse> Items { get; init; } = [];

  public static WishlistResponse From(Wishlist w) => new()
  {
    Id = w.Id,
    UserId = w.UserId,
    Items = w.Items.Select(WishlistItemResponse.From),
  };
}
