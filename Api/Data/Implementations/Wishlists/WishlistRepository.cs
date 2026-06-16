using Api.Context;
using Api.Data.Interfaces.Wishlists;
using Api.Models.Entities.Wishlists;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Wishlists;

public class WishlistRepository(DatabaseContext context) : IWishlistRepository
{
  public async Task<Wishlist> CreateWishlistAsync(Wishlist wishlist)
  {
    context.Wishlists.Add(wishlist);
    await context.SaveChangesAsync();
    return wishlist;
  }

  public async Task<bool> DeleteWishlistAsync(Guid wishlistId)
  {
    var wishlist = await GetWishlistByIdAsync(wishlistId);
    if (wishlist == null) return false;

    context.Wishlists.Remove(wishlist);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<Wishlist?> GetWishlistByIdAsync(Guid wishlistId)
  {
    return await context.Wishlists.Include(w => w.Items).FirstOrDefaultAsync(w => w.Id == wishlistId);
  }

  public async Task<Wishlist?> GetWishlistByUserIdAsync(Guid userId)
  {
    return await context.Wishlists.Include(w => w.Items).FirstOrDefaultAsync(w => w.UserId == userId);
  }

  public async Task<Wishlist?> UpdateWishlistAsync(Wishlist wishlist)
  {
    context.Wishlists.Update(wishlist);
    await context.SaveChangesAsync();
    return wishlist;
  }
}