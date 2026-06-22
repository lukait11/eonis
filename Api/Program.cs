using System.Text;
using Api.Context;
using Api.Data;
using Api.Data.Implementations.Catalog;
using Api.Data.Implementations.Identity;
using Api.Data.Implementations.Orders;
using Api.Data.Implementations.Reviews;
using Api.Data.Implementations.Shopping;
using Api.Data.Implementations.Wishlists;
using Api.Data.Interfaces.Catalog;
using Api.Data.Interfaces.Identity;
using Api.Data.Interfaces.Orders;
using Api.Data.Interfaces.Reviews;
using Api.Data.Interfaces.Shopping;
using Api.Data.Interfaces.Wishlists;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
  .AddJsonOptions(opts =>
  {
    opts.JsonSerializerOptions.Converters.Add(
      new System.Text.Json.Serialization.JsonStringEnumConverter());
    opts.JsonSerializerOptions.ReferenceHandler =
      System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
  });
builder.Services.AddDbContext<DatabaseContext>();

var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]
    ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? ["http://server1:4210"];

builder.Services.AddCors(options =>
  options.AddPolicy("Frontend", policy =>
    policy.WithOrigins(allowedOrigins)
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials()));

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

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<ISellerProfileRepository, SellerProfileRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
builder.Services.AddScoped<ISellerReviewRepository, SellerReviewRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IWishlistItemRepository, WishlistItemRepository>();

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<IStorageService, GarageStorageService>();


builder.Services.AddAuthorization();

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
  var context = scope.ServiceProvider
    .GetRequiredService<DatabaseContext>();

  // Apply migrations and seed data
  await context.Database.EnsureCreatedAsync();

  // Only seed if database is empty
  if (!await context.Categories.AnyAsync())
  {
    await SeedData.InitializeAsync(context);
  }
}

app.UseCors("Frontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();