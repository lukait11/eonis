using Api.Context;
using Api.Data.Interfaces.Identity;
using Api.Models.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Identity;

public class ApplicationUserRepository(DatabaseContext context) : IApplicationUserRepository
{
  public async Task<ApplicationUser> CreateUserAsync(ApplicationUser user)
  {
    user.CreatedAt = DateTime.UtcNow;
    user.UpdatedAt = DateTime.UtcNow;
    context.Users.Add(user);
    await context.SaveChangesAsync();
    return user;
  }

  public async Task<bool> DeleteUserAsync(Guid userId)
  {
    var user = await GetUserByIdAsync(userId);
    if (user == null) return false;

    context.Users.Remove(user);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<ApplicationUser?> GetUserByIdAsync(Guid userId)
  {
    return await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
  }

  public async Task<IEnumerable<ApplicationUser>> GetUsersAsync()
  {
    return await context.Users.ToListAsync();
  }

  public async Task<ApplicationUser?> UpdateUserAsync(ApplicationUser user)
  {
    user.UpdatedAt = DateTime.UtcNow;
    context.Users.Update(user);
    await context.SaveChangesAsync();
    return user;
  }

}
