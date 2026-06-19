using System.Security.Cryptography;
using Api.Data.Interfaces.Identity;

namespace Api.Services;

public class RefreshTokenService(IApplicationUserRepository userRepository)
{
  public async Task<string> GenerateRefreshTokenAsync(Guid userId)
  {
    var token = Convert
      .ToBase64String(RandomNumberGenerator.GetBytes(32))
      .Replace("+", "-")
      .Replace("/", "_")
      .TrimEnd('=');
    var user = await userRepository.GetUserByIdAsync(userId);
    user!.RefreshToken = token;
    await userRepository.UpdateUserAsync(user);
    return token;
  }

  public async Task<(Guid userId, string newToken)?> ValidateRefreshTokenAsync(Guid userId, string token)
  {
    var user = await userRepository.GetUserByIdAsync(userId);
    if (user is null) return null;

    if (user.RefreshToken != token) return null;

    var newToken = await GenerateRefreshTokenAsync(user.Id);
    user.RefreshToken = newToken;
    await userRepository.UpdateUserAsync(user);
    return (user.Id, newToken);
  }

  public async Task InvalidateRefreshTokenAsync(Guid userId)
  {
    var user = await userRepository.GetUserByIdAsync(userId);
    if (user is null) return;

    user.RefreshToken = null;
    await userRepository.UpdateUserAsync(user);
  }
}
