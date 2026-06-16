using Api.Models.Entities.Orders;

namespace Api.Data.Interfaces.Orders;

public interface IOrderRepository
{
  Task<IEnumerable<Order>> GetOrdersAsync();
  Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
  Task<IEnumerable<Order>> GetOrdersBySellerIdAsync(Guid sellerId);
  Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
  Task<Order?> GetOrderByIdAsync(Guid orderId);
  Task<Order> CreateOrderAsync(Order order);
  Task<Order?> UpdateOrderAsync(Order order);
  Task<bool> DeleteOrderAsync(Guid orderId);
}
