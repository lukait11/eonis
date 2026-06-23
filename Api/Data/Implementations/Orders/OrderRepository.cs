using Api.Context;
using Api.Data.Interfaces.Orders;
using Api.Models.Entities.Orders;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Orders;

public class OrderRepository(DatabaseContext context) : IOrderRepository
{
  public async Task<Order> CreateOrderAsync(Order order)
  {
    order.CreatedAt = DateTime.UtcNow;
    context.Orders.Add(order);
    await context.SaveChangesAsync();
    return order;
  }

  public async Task<bool> DeleteOrderAsync(Guid orderId)
  {
    var order = await GetOrderByIdAsync(orderId);
    if (order == null) return false;

    context.Orders.Remove(order);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<Order?> GetOrderByIdAsync(Guid orderId)
  {
    return await context.Orders
      .Include(o => o.Address)
      .Include(o => o.Items).ThenInclude(i => i.ProductVariant)
      .FirstOrDefaultAsync(o => o.Id == orderId);
  }

  public async Task<IEnumerable<Order>> GetOrdersAsync()
  {
    return await context.Orders.Include(o => o.Items).ToListAsync();
  }

  public async Task<IEnumerable<Order>> GetOrdersBySellerIdAsync(Guid sellerId)
  {
    return await context.Orders.Include(o => o.Items).Where(o => o.SellerProfileId == sellerId).ToListAsync();
  }

  public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
  {
    return await context.Orders.Include(o => o.Items).Where(o => o.Status == status).ToListAsync();
  }

  public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
  {
    return await context.Orders.Include(o => o.Items).Where(o => o.UserId == userId).ToListAsync();
  }

  public async Task<Order?> UpdateOrderAsync(Order order)
  {
    context.Update(order);
    await context.SaveChangesAsync();
    return order;
  }
}
