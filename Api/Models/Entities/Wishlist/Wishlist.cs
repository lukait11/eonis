using Api.Models.Entities.Identity;

namespace Api.Models.Entities.Wishlist;

public class Wishlist
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }

  // Navigation properties
  public ApplicationUser? User { get; set; }
  public ICollection<WishlistItem> Items { get; set; } = [];
}
