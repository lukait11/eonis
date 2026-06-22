using System.Text.Json.Serialization;
using Api.Models.Entities.Catalog;

namespace Api.Models.Entities.Orders;

public class OrderItem
{
  public Guid Id { get; set; }
  public Guid OrderId { get; set; }
  public Guid ProductVariantId { get; set; }
  public int Quantity { get; set; }

  // Navigation properties
  [JsonIgnore]
  public Order? Order { get; set; }
  public ProductVariant? ProductVariant { get; set; }
}