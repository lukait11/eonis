using Api.Models.Entities.Wishlists;

namespace Api.Data.Interfaces.Wishlists;

public interface IWishlistRepository
{
  Task<Wishlist?> GetWishlistByIdAsync(Guid wishlistId);
  Task<Wishlist?> GetWishlistByUserIdAsync(Guid userId);
  Task<Wishlist> CreateWishlistAsync(Wishlist wishlist);
  Task<Wishlist?> UpdateWishlistAsync(Wishlist wishlist);
  Task<bool> DeleteWishlistAsync(Guid wishlistId);
}
