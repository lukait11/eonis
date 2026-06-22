using System.Text.Json.Serialization;

namespace Api.Models.Entities.Catalog;

public class Category
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public Guid? ParentCategoryId { get; set; }

  // Navigation properties
  [JsonIgnore]
  public Category? ParentCategory { get; set; } 
}