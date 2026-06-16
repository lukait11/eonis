using Api.Models.Entities.Shopping;

namespace Api.Data.Interfaces.Shopping;

public interface ICartRepository
{
  Task<Cart?> GetCartByIdAsync(Guid cartId);
  Task<Cart?> GetCartByUserIdAsync(Guid userId);
  Task<Cart> CreateCartAsync(Cart cart);
  Task<Cart?> UpdateCartAsync(Cart cart);
  Task<bool> DeleteCartAsync(Guid cartId);
}
