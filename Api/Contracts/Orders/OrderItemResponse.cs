using Api.Contracts.Catalog;
using Api.Models.Entities.Orders;

namespace Api.Contracts.Orders;

public class OrderItemResponse
{
  public Guid Id { get; init; }
  public Guid OrderId { get; init; }
  public Guid ProductVariantId { get; init; }
  public int Quantity { get; init; }
  public ProductVariantResponse? ProductVariant { get; init; }

  public static OrderItemResponse From(OrderItem i) => new()
  {
    Id = i.Id,
    OrderId = i.OrderId,
    ProductVariantId = i.ProductVariantId,
    Quantity = i.Quantity,
    ProductVariant = i.ProductVariant != null ? ProductVariantResponse.From(i.ProductVariant) : null,
  };
}
