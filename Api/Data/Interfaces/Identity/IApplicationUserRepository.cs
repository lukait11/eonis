using Api.Models.Entities.Identity;

namespace Api.Data.Interfaces.Identity;

public interface IApplicationUserRepository
{
  Task<IEnumerable<ApplicationUser>> GetUsersAsync();
  Task<ApplicationUser?> GetUserByIdAsync(Guid userId);
  Task<ApplicationUser> CreateUserAsync(ApplicationUser user);
  Task<ApplicationUser?> UpdateUserAsync(ApplicationUser user);
  Task<bool> DeleteUserAsync(Guid userId);
  Task<string?> UploadProfilePictureAsync(Guid userId, IFormFile file);
}
