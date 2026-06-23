using Api.Models.Entities.Orders;

namespace Api.Models.DTO.Orders;

public class UpdateOrderStatusRequest
{
  public Guid Id { get; set; }
  public OrderStatus Status { get; set; }
}
