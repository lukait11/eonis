using Api.Models.Entities.Shopping;

namespace Api.Data.Interfaces.Shopping;

public interface ICartItemRepository
{
  Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId);
  Task<IEnumerable<CartItem>> GetCartItemsByCartIdAsync(Guid cartId);
  Task<CartItem> CreateCartItemAsync(CartItem cartItem);
  Task<CartItem?> UpdateCartItemAsync(CartItem cartItem);
  Task<bool> DeleteCartItemAsync(Guid cartItemId);
  Task DeleteCartItemsByCartIdAsync(Guid cartId);
}
