using Api.Models.Entities.Identity;

namespace Api.Models.DTO.Auth;

public class RegisterRequest
{
  public string Email { get; set; } = null!;
  public string Password { get; set; } = null!;
  public string FirstName { get; set; } = null!;
  public string LastName { get; set; } = null!;
  public UserRole Role { get; set; }
}
