using Microsoft.AspNetCore.Identity;

namespace Api.Models.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? ProfilePictureUrl { get; set; }
  public DateTime? DateOfBirth { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
  public DateTime? LastLoginAt { get; set; }
  public bool IsActive { get; set; } = true;

   // Navigation properties
  public SellerProfile? SellerProfile { get; set; }
}
