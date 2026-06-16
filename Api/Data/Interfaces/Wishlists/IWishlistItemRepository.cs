using Api.Models.Entities.Wishlists;

namespace Api.Data.Interfaces.Wishlists;

public interface IWishlistItemRepository
{
  Task<WishlistItem?> GetWishlistItemByIdAsync(Guid wishlistItemId);
  Task<IEnumerable<WishlistItem>> GetWishlistItemsByWishlistIdAsync(Guid wishlistId);
  Task<WishlistItem> CreateWishlistItemAsync(WishlistItem wishlistItem);
  Task<WishlistItem?> UpdateWishlistItemAsync(WishlistItem wishlistItem);
  Task<bool> DeleteWishlistItemAsync(Guid wishlistItemId);
}
