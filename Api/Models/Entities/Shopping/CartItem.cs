using System.Text.Json.Serialization;
using Api.Models.Entities.Catalog;

namespace Api.Models.Entities.Shopping;

public class CartItem
{
  public Guid Id { get; set; }
  public Guid CartId { get; set; }
  public Guid ProductVariantId { get; set; }
  public int Quantity { get; set; }

  // Navigation properties
  [JsonIgnore]
  public Cart? Cart { get; set; }
  public ProductVariant? ProductVariant { get; set; }
}
