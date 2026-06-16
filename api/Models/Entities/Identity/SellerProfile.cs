using Api.Models.Entities.Catalog;

namespace Api.Models.Entities.Identity;

public class SellerProfile
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string? StoreName { get; set; }
  public string? Description { get; set; }

  // Navigation properties
  public ICollection<Product> Products { get; set; } = [];
  public ApplicationUser? User { get; set; }
}
