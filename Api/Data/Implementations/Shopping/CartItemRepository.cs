using Api.Context;
using Api.Data.Interfaces.Shopping;
using Api.Models.Entities.Shopping;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Shopping;

public class CartItemRepository(DatabaseContext context) : ICartItemRepository
{
  public async Task<CartItem> CreateCartItemAsync(CartItem cartItem)
  {
    context.CartItems.Add(cartItem);
    await context.SaveChangesAsync();
    return cartItem;
  }

  public async Task<bool> DeleteCartItemAsync(Guid cartItemId)
  {
    var cartItem = await GetCartItemByIdAsync(cartItemId);
    if (cartItem == null) return false;

    context.CartItems.Remove(cartItem);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId)
  {
    return await context.CartItems.FirstOrDefaultAsync(ci => ci.Id == cartItemId);
  }

  public async Task<IEnumerable<CartItem>> GetCartItemsByCartIdAsync(Guid cartId)
  {
    return await context.CartItems
      .Include(ci => ci.ProductVariant)
      .Where(ci => ci.CartId == cartId)
      .ToListAsync();
  }

  public async Task<CartItem?> UpdateCartItemAsync(CartItem cartItem)
  {
    var existing = await context.CartItems.FindAsync(cartItem.Id);
    if (existing == null) return null;
    existing.Quantity = cartItem.Quantity;
    await context.SaveChangesAsync();
    return existing;
  }
}