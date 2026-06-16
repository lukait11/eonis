using System.Text;
using Api.Context;
using Api.Data;
using Api.Models.Entities.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services
  .AddIdentity<ApplicationUser, IdentityRole<Guid>>()
  .AddEntityFrameworkStores<DatabaseContext>()
  .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,

    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],

    IssuerSigningKey =
      new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
      )
  };
});

builder.Services.AddAuthorization();

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
  var context = scope.ServiceProvider
    .GetRequiredService<DatabaseContext>();
  var userManager = scope.ServiceProvider
    .GetRequiredService<UserManager<ApplicationUser>>();
  var roleManager = scope.ServiceProvider
    .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

  // Apply migrations and seed data
  await context.Database.EnsureCreatedAsync();

  // Only seed if database is empty
  if (!await context.Categories.AnyAsync())
  {
    await SeedData.InitializeAsync(context, userManager, roleManager);
  }
}

app.UseHttpsRedirection();

app.Run();

