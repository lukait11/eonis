using Api.Context;
using Api.Data.Interfaces.Shopping;
using Api.Models.Entities.Shopping;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Shopping;

public class CartRepository(DatabaseContext context) : ICartRepository
{
  public async Task<Cart> CreateCartAsync(Cart cart)
  {
    context.Carts.Add(cart);
    await context.SaveChangesAsync();
    return cart;
  }

  public async Task<bool> DeleteCartAsync(Guid cartId)
  {
    var cart = await GetCartByIdAsync(cartId);
    if (cart == null) return false;

    context.Carts.Remove(cart);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<Cart?> GetCartByIdAsync(Guid cartId)
  {
    return await context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == cartId);
  }

  public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
  {
    return await context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
  }

  public async Task<Cart?> UpdateCartAsync(Cart cart)
  {
    context.Carts.Update(cart);
    await context.SaveChangesAsync();
    return cart;
  }
}