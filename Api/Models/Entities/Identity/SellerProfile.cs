using System.Text.Json.Serialization;
using Api.Models.Entities.Catalog;

namespace Api.Models.Entities.Identity;

public class SellerProfile
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string? StoreName { get; set; }
  public string? Description { get; set; }
  public string ? LogoUrl { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
  public bool IsActive { get; set; } = true;

  // Navigation properties
  public ICollection<Product> Products { get; set; } = [];
  
  [JsonIgnore]
  public ApplicationUser? User { get; set; }
}
