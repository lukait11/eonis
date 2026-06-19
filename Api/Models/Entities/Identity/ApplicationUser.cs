namespace Api.Models.Entities.Identity;

public class ApplicationUser
{
  public Guid Id { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Email { get; set; }
  public UserRole Role { get; set; } = UserRole.Customer;
  public string? RefreshToken { get; set; }
  public string? PhoneNumber { get; set; }
  public string? PasswordHash { get; set; }
  public string? ProfilePictureUrl { get; set; }
  public DateTime? DateOfBirth { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
  public DateTime? LastLoginAt { get; set; }
  public bool IsActive { get; set; } = true;

   // Navigation properties
  public SellerProfile? SellerProfile { get; set; }
}
