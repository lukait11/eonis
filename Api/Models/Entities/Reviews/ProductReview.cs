using Api.Models.Entities.Catalog;
using Api.Models.Entities.Identity;

namespace Api.Models.Entities.Reviews;

public class ProductReview
{
  public Guid Id { get; set; }
  public Guid ProductId { get; set; }
  public Guid UserId { get; set; }
  public int Rating { get; set; }
  public string? Comment { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  public Product? Product { get; set; }
  public ApplicationUser? User { get; set; }
}
