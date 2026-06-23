namespace Api.Models.DTO.Identity;

public class UpdateUserRequest
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? PhoneNumber { get; set; }
  public DateTime? DateOfBirth { get; set; }
}
