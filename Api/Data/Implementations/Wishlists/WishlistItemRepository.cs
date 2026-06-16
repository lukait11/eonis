using Api.Context;
using Api.Data.Interfaces.Wishlists;
using Api.Models.Entities.Wishlists;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Wishlists;

public class WishlistItemRepository(DatabaseContext context) : IWishlistItemRepository
{
  public async Task<WishlistItem> CreateWishlistItemAsync(WishlistItem wishlistItem)
  {
    wishlistItem.AddedAt = DateTime.UtcNow;
    context.WishlistItems.Add(wishlistItem);
    await context.SaveChangesAsync();
    return wishlistItem;
  }

  public async Task<bool> DeleteWishlistItemAsync(Guid wishlistItemId)
  {
    var wishlistItem = await GetWishlistItemByIdAsync(wishlistItemId);
    if (wishlistItem == null) return false;

    context.WishlistItems.Remove(wishlistItem);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<WishlistItem?> GetWishlistItemByIdAsync(Guid wishlistItemId)
  {
    return await context.WishlistItems.FirstOrDefaultAsync(w => w.Id == wishlistItemId);
  }

  public async Task<IEnumerable<WishlistItem>> GetWishlistItemsByWishlistIdAsync(Guid wishlistId)
  {
    return await context.WishlistItems.Where(w => w.WishlistId == wishlistId).ToListAsync();
  }

  public async Task<WishlistItem?> UpdateWishlistItemAsync(WishlistItem wishlistItem)
  {
    context.WishlistItems.Update(wishlistItem);
    await context.SaveChangesAsync();
    return wishlistItem;
  }

}
