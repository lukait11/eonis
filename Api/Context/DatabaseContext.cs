using Api.Models.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api.Context;
public class DatabaseContext(
  DbContextOptions<DatabaseContext> options,
  IConfiguration configuration
) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
  private readonly IConfiguration _configuration = configuration;
}