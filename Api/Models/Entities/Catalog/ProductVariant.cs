using System.Text.Json.Serialization;

namespace Api.Models.Entities.Catalog;

public class ProductVariant
{
  public Guid Id { get; set; }
  public Guid ProductId { get; set; }
  public string? Size { get; set; }
  public string? Color { get; set; }
  public int Quantity { get; set; }

  // Navigation properties
  [JsonIgnore]
  public Product? Product { get; set; }
  public IEnumerable<ProductImage>? Images { get; set; }
}
