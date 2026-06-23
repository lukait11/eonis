using Api.Models.Entities.Identity;

namespace Api.Contracts.Identity;

public class SellerProfileResponse
{
  public Guid Id { get; init; }
  public Guid UserId { get; init; }
  public string? StoreName { get; init; }
  public string? Description { get; init; }
  public string? ProfilePictureUrl { get; init; }
  public DateTime CreatedAt { get; init; }
  public DateTime? UpdatedAt { get; init; }
  public bool IsActive { get; init; }

  public static SellerProfileResponse From(SellerProfile s) => new()
  {
    Id = s.Id,
    UserId = s.UserId,
    StoreName = s.StoreName,
    Description = s.Description,
    ProfilePictureUrl = s.ProfilePictureUrl,
    CreatedAt = s.CreatedAt,
    UpdatedAt = s.UpdatedAt,
    IsActive = s.IsActive,
  };
}
