using Api.Models.Entities.Orders;

namespace Api.Models.DTO.Orders;

public class CreateOrderRequest
{
  public Guid UserId { get; set; }
  public Guid SellerProfileId { get; set; }
  public Guid AddressId { get; set; }
  public double BaseAmount { get; set; }
  public double Discount { get; set; }
  public OrderStatus Status { get; set; } = OrderStatus.Pending;
}
