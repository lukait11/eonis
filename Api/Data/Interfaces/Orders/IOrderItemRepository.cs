using Api.Models.Entities.Orders;

namespace Api.Data.Interfaces.Orders;

public interface IOrderItemRepository
{
  Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId);
  Task<OrderItem?> GetOrderItemByIdAsync(Guid orderItemId);
  Task<OrderItem> CreateOrderItemAsync(OrderItem orderItem);
  Task<OrderItem?> UpdateOrderItemAsync(OrderItem orderItem);
  Task<bool> DeleteOrderItemAsync(Guid orderItemId);
}
