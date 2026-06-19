using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Models.Entities.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services;

public class JwtService (IConfiguration configuration)
{
  public string GenerateToken(Guid userId, string email, UserRole role)
  {
    var claims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
      new Claim(ClaimTypes.Email, email),
      new Claim(ClaimTypes.Role, role.ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: configuration["Jwt:Issuer"],
      audience: configuration["Jwt:Audience"],
      claims: claims,
      expires: DateTime.Now.AddMinutes(configuration.GetValue("Jwt:ExpirationMinutes", 15.0)),
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
