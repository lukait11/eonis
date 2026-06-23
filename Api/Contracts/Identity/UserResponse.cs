using Api.Models.Entities.Identity;

namespace Api.Contracts.Identity;

public class UserResponse
{
  public Guid Id { get; init; }
  public string? Email { get; init; }
  public string? FirstName { get; init; }
  public string? LastName { get; init; }
  public string? PhoneNumber { get; init; }
  public DateTime? DateOfBirth { get; init; }
  public UserRole Role { get; init; }
  public string? ProfilePictureUrl { get; init; }
  public DateTime CreatedAt { get; init; }
  public DateTime? UpdatedAt { get; init; }
  public bool IsActive { get; init; }

  public static UserResponse From(ApplicationUser u) => new()
  {
    Id = u.Id,
    Email = u.Email,
    FirstName = u.FirstName,
    LastName = u.LastName,
    PhoneNumber = u.PhoneNumber,
    DateOfBirth = u.DateOfBirth,
    Role = u.Role,
    ProfilePictureUrl = u.ProfilePictureUrl,
    CreatedAt = u.CreatedAt,
    UpdatedAt = u.UpdatedAt,
    IsActive = u.IsActive,
  };
}
