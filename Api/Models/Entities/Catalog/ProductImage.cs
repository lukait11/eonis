using System.Text.Json.Serialization;

namespace Api.Models.Entities.Catalog;

public class ProductImage
{
  public Guid Id { get; set; }
  public Guid ProductId { get; set; }
  public string? ImageUrl { get; set; }
  public bool IsPrimary { get; set; }
  
  // Navigation properties
  [JsonIgnore]
  public Product? Product { get; set; }
}
