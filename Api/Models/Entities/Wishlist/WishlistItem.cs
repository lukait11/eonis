using Api.Models.Entities.Catalog;

namespace Api.Models.Entities.Wishlist;

public class WishlistItem
{
  public Guid Id { get; set; }
  public Guid WishlistId { get; set; }
  public Guid ProductId { get; set; }
  public DateTime AddedAt { get; set; }

  // Navigation properties
  public Wishlist? Wishlist { get; set; }
  public Product? Product { get; set; }
}