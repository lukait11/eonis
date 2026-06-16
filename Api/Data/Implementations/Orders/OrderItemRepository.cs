using Api.Context;
using Api.Data.Interfaces.Orders;
using Api.Models.Entities.Orders;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Orders;

public class OrderItemRepository(DatabaseContext context) : IOrderItemRepository
{
  public async Task<OrderItem> CreateOrderItemAsync(OrderItem orderItem)
  {
    context.OrderItems.Add(orderItem);
    await context.SaveChangesAsync();
    return orderItem;
  }

  public async Task<bool> DeleteOrderItemAsync(Guid orderItemId)
  {
    var orderItem = await GetOrderItemByIdAsync(orderItemId);
    if (orderItem == null) return false;

    context.OrderItems.Remove(orderItem);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<OrderItem?> GetOrderItemByIdAsync(Guid orderItemId)
  {
    return await context.OrderItems.FirstOrDefaultAsync(oi => oi.Id == orderItemId);
  }

  public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId)
  {
    return await context.OrderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
  }

  public async Task<OrderItem?> UpdateOrderItemAsync(OrderItem orderItem)
  {
    context.Update(orderItem);
    await context.SaveChangesAsync();
    return orderItem;
  }
}