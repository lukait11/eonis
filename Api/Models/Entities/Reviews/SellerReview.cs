using Api.Models.Entities.Identity;

namespace Api.Models.Entities.Reviews;

public class SellerReview
{
  public Guid Id { get; set; }
  public Guid SellerProfileId { get; set; }
  public Guid UserId { get; set; }
  public int Rating { get; set; }
  public string? Comment { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  public SellerProfile? SellerProfile { get; set; }
  public ApplicationUser? User { get; set; }
}