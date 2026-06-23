using System.Text.Json.Serialization;

namespace Api.Models.Entities.Catalog;

public class Category
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }

  // Navigation properties
  [JsonIgnore]
  public ICollection<Product> Products { get; set; } = [];
}
