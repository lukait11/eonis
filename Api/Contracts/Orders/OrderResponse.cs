using Api.Models.Entities.Orders;

namespace Api.Contracts.Orders;

public class OrderResponse
{
  public Guid Id { get; init; }
  public Guid UserId { get; init; }
  public Guid SellerProfileId { get; init; }
  public Guid AddressId { get; init; }
  public OrderStatus Status { get; init; }
  public double BaseAmount { get; init; }
  public double Discount { get; init; }
  public DateTime CreatedAt { get; init; }
  public AddressResponse? Address { get; init; }
  public IEnumerable<OrderItemResponse> Items { get; init; } = [];

  public static OrderResponse From(Order o) => new()
  {
    Id = o.Id,
    UserId = o.UserId,
    SellerProfileId = o.SellerProfileId,
    AddressId = o.AddressId,
    Status = o.Status,
    BaseAmount = o.BaseAmount,
    Discount = o.Discount,
    CreatedAt = o.CreatedAt,
    Address = o.Address != null ? AddressResponse.From(o.Address) : null,
    Items = o.Items.Select(OrderItemResponse.From),
  };
}
