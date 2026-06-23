using Api.Context;
using Api.Models.Entities.Catalog;
using Api.Models.Entities.Identity;
using Api.Models.Entities.Orders;
using Api.Models.Entities.Payments;
using Api.Models.Entities.Reviews;
using Api.Models.Entities.Shopping;
using Api.Models.Entities.Wishlists;

namespace Api.Data;

public static class SeedData
{
  public static async Task InitializeAsync(DatabaseContext context)
  {
    // Users
    var seller = new ApplicationUser
    {
      Id = Guid.NewGuid(),
      Role = UserRole.Seller,
      Email = "seller@example.com",
      PasswordHash = BCrypt.Net.BCrypt.HashPassword("Seller123!"),
      FirstName = "John",
      LastName = "Smith",
      IsActive = true,
      CreatedAt = DateTime.UtcNow
    };

    var customer1 = new ApplicationUser
    {
      Id = Guid.NewGuid(),
      Role = UserRole.Customer,
      Email = "customer1@example.com",
      PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!"),
      FirstName = "Jane",
      LastName = "Doe",
      IsActive = true,
      CreatedAt = DateTime.UtcNow
    };

    var customer2 = new ApplicationUser
    {
      Id = Guid.NewGuid(),
      Role = UserRole.Customer,
      Email = "customer2@example.com",
      PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!"),
      FirstName = "Bob",
      LastName = "Johnson",
      IsActive = true,
      CreatedAt = DateTime.UtcNow
    };

    context.Users.AddRange(seller, customer1, customer2);

    // Seller profile
    var sellerProfile = new SellerProfile
    {
      Id = Guid.NewGuid(),
      UserId = seller.Id,
      StoreName = "StyleHub",
      Description = "Premium clothing and fashion for all"
    };

    context.SellerProfiles.Add(sellerProfile);

    // Flat categories
    var catMen        = new Category { Id = Guid.NewGuid(), Name = "Men",        Description = "Men's clothing and accessories" };
    var catWomen      = new Category { Id = Guid.NewGuid(), Name = "Women",      Description = "Women's clothing and accessories" };
    var catKids       = new Category { Id = Guid.NewGuid(), Name = "Kids",       Description = "Children's clothing" };
    var catTShirts    = new Category { Id = Guid.NewGuid(), Name = "T-Shirts",   Description = "T-shirts and casual tops" };
    var catHoodies    = new Category { Id = Guid.NewGuid(), Name = "Hoodies",    Description = "Hoodies and sweatshirts" };
    var catJeans      = new Category { Id = Guid.NewGuid(), Name = "Jeans",      Description = "Jeans and denim" };
    var catCapsHats   = new Category { Id = Guid.NewGuid(), Name = "Caps & Hats", Description = "Caps and hats" };

    context.Categories.AddRange(catMen, catWomen, catKids, catTShirts, catHoodies, catJeans, catCapsHats);
    context.SaveChanges();

    // Products — each product carries multiple flat categories
    var menTshirt = new Product
    {
      Id = Guid.NewGuid(),
      SellerId = sellerProfile.Id,
      Name = "Premium Cotton T-Shirt",
      Description = "Comfortable and stylish 100% cotton t-shirt for everyday wear",
      BasePrice = 29.99,
      Discount = 15,
      Material = "100% Cotton",
      Status = ProductStatus.Available,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow,
      Categories = [catMen, catTShirts],
    };

    var menHoodie = new Product
    {
      Id = Guid.NewGuid(),
      SellerId = sellerProfile.Id,
      Name = "Casual Hoodie",
      Description = "Warm and cozy hoodie perfect for cold weather",
      BasePrice = 59.99,
      Discount = 20,
      Material = "Cotton Blend",
      Status = ProductStatus.Available,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow,
      Categories = [catMen, catHoodies],
    };

    var menCap = new Product
    {
      Id = Guid.NewGuid(),
      SellerId = sellerProfile.Id,
      Name = "Classic Baseball Cap",
      Description = "Adjustable baseball cap with embroidered logo",
      BasePrice = 24.99,
      Discount = 10,
      Material = "Cotton Canvas",
      Status = ProductStatus.Available,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow,
      Categories = [catMen, catCapsHats],
    };

    var womenTshirt = new Product
    {
      Id = Guid.NewGuid(),
      SellerId = sellerProfile.Id,
      Name = "Fitted V-Neck T-Shirt",
      Description = "Elegant fitted t-shirt with v-neckline",
      BasePrice = 34.99,
      Discount = 25,
      Material = "100% Cotton",
      Status = ProductStatus.Available,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow,
      Categories = [catWomen, catTShirts],
    };

    var womenJeans = new Product
    {
      Id = Guid.NewGuid(),
      SellerId = sellerProfile.Id,
      Name = "Slim Fit Denim Jeans",
      Description = "Classic slim fit jeans with perfect stretch",
      BasePrice = 79.99,
      Discount = 30,
      Material = "98% Cotton, 2% Elastane",
      Status = ProductStatus.Available,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow,
      Categories = [catWomen, catJeans],
    };

    context.Products.AddRange(menTshirt, menHoodie, menCap, womenTshirt, womenJeans);
    context.SaveChanges();

    // Product images
    context.ProductImages.AddRange(
      new ProductImage { Id = Guid.NewGuid(), ProductId = menTshirt.Id, ImageUrl = "https://via.placeholder.com/500?text=Men+T-Shirt+Front", IsPrimary = true },
      new ProductImage { Id = Guid.NewGuid(), ProductId = menTshirt.Id, ImageUrl = "https://via.placeholder.com/500?text=Men+T-Shirt+Back",  IsPrimary = false },
      new ProductImage { Id = Guid.NewGuid(), ProductId = menHoodie.Id, ImageUrl = "https://via.placeholder.com/500?text=Hoodie+Front",       IsPrimary = true },
      new ProductImage { Id = Guid.NewGuid(), ProductId = menCap.Id,    ImageUrl = "https://via.placeholder.com/500?text=Baseball+Cap",       IsPrimary = true },
      new ProductImage { Id = Guid.NewGuid(), ProductId = womenTshirt.Id, ImageUrl = "https://via.placeholder.com/500?text=Women+V-Neck+Front", IsPrimary = true },
      new ProductImage { Id = Guid.NewGuid(), ProductId = womenJeans.Id,  ImageUrl = "https://via.placeholder.com/500?text=Women+Jeans+Front",  IsPrimary = true }
    );

    // Product variants
    context.ProductVariants.AddRange(
      new ProductVariant { Id = Guid.NewGuid(), ProductId = menTshirt.Id, Size = "S",   Color = "Black", Quantity = 100 },
      new ProductVariant { Id = Guid.NewGuid(), ProductId = menTshirt.Id, Size = "M",   Color = "Black", Quantity = 150 },
      new ProductVariant { Id = Guid.NewGuid(), ProductId = menTshirt.Id, Size = "L",   Color = "White", Quantity = 120 },
      new ProductVariant { Id = Guid.NewGuid(), ProductId = menTshirt.Id, Size = "XL",  Color = "Navy",  Quantity = 80  },

      new ProductVariant { Id = Guid.NewGuid(), ProductId = menHoodie.Id, Size = "S",   Color = "Gray",  Quantity = 50 },
      new ProductVariant { Id = Guid.NewGuid(), ProductId = menHoodie.Id, Size = "M",   Color = "Gray",  Quantity = 70 },
      new ProductVariant { Id = Guid.NewGuid(), ProductId = menHoodie.Id, Size = "L",   Color = "Black", Quantity = 60 },

      new ProductVariant { Id = Guid.NewGuid(), ProductId = menCap.Id, Size = "One Size", Color = "Black", Quantity = 200 },
      new ProductVariant { Id = Guid.NewGuid(), ProductId = menCap.Id, Size = "One Size", Color = "Navy",  Quantity = 180 },
      new ProductVariant { Id = Guid.NewGuid(), ProductId = menCap.Id, Size = "One Size", Color = "Red",   Quantity = 150 },

      new ProductVariant { Id = Guid.NewGuid(), ProductId = womenTshirt.Id, Size = "XS", Color = "White", Quantity = 90  },
      new ProductVariant { Id = Guid.NewGuid(), ProductId = womenTshirt.Id, Size = "S",  Color = "Pink",  Quantity = 110 },
      new ProductVariant { Id = Guid.NewGuid(), ProductId = womenTshirt.Id, Size = "M",  Color = "Pink",  Quantity = 130 },

      new ProductVariant { Id = Guid.NewGuid(), ProductId = womenJeans.Id, Size = "24", Color = "Dark Blue",  Quantity = 60 },
      new ProductVariant { Id = Guid.NewGuid(), ProductId = womenJeans.Id, Size = "26", Color = "Dark Blue",  Quantity = 80 },
      new ProductVariant { Id = Guid.NewGuid(), ProductId = womenJeans.Id, Size = "28", Color = "Light Blue", Quantity = 75 }
    );

    // Product reviews
    context.ProductReviews.AddRange(
      new ProductReview { Id = Guid.NewGuid(), ProductId = menTshirt.Id, UserId = customer1.Id, Rating = 5, Comment = "Great quality cotton! Very comfortable and fits well.", CreatedAt = DateTime.UtcNow },
      new ProductReview { Id = Guid.NewGuid(), ProductId = menTshirt.Id, UserId = customer2.Id, Rating = 4, Comment = "Good quality, but slightly smaller than expected.", CreatedAt = DateTime.UtcNow },
      new ProductReview { Id = Guid.NewGuid(), ProductId = menHoodie.Id, UserId = customer1.Id, Rating = 5, Comment = "Perfect hoodie for the season! Warm and cozy.", CreatedAt = DateTime.UtcNow },
      new ProductReview { Id = Guid.NewGuid(), ProductId = womenJeans.Id, UserId = customer2.Id, Rating = 4, Comment = "Excellent fit and very comfortable!", CreatedAt = DateTime.UtcNow }
    );

    // Addresses
    var address1 = new Address { Id = Guid.NewGuid(), UserId = customer1.Id, Street = "123 Main St",  City = "New York",    Country = "USA", PostalCode = 10001 };
    var address2 = new Address { Id = Guid.NewGuid(), UserId = customer2.Id, Street = "456 Oak Ave",  City = "Los Angeles", Country = "USA", PostalCode = 90001 };
    context.Addresses.AddRange(address1, address2);

    // Orders
    var order1 = new Order { Id = Guid.NewGuid(), UserId = customer1.Id, AddressId = address1.Id, Status = OrderStatus.Delivered, CreatedAt = DateTime.UtcNow.AddDays(-5), BaseAmount = 89.98, Discount = 13.50 };
    var order2 = new Order { Id = Guid.NewGuid(), UserId = customer2.Id, AddressId = address2.Id, Status = OrderStatus.Pending,   CreatedAt = DateTime.UtcNow.AddDays(-1), BaseAmount = 79.99, Discount = 24.00 };
    context.Orders.AddRange(order1, order2);

    // Order items — reference the first variants seeded above; we need their IDs
    // We'll query them by product ID after the SaveChanges above
    context.SaveChanges();

    var menTshirtVariants  = context.ProductVariants.Where(v => v.ProductId == menTshirt.Id).ToList();
    var menCapVariants     = context.ProductVariants.Where(v => v.ProductId == menCap.Id).ToList();
    var womenJeansVariants = context.ProductVariants.Where(v => v.ProductId == womenJeans.Id).ToList();
    var menHoodieVariants  = context.ProductVariants.Where(v => v.ProductId == menHoodie.Id).ToList();
    var womenTshirtVariants = context.ProductVariants.Where(v => v.ProductId == womenTshirt.Id).ToList();

    context.OrderItems.AddRange(
      new OrderItem { Id = Guid.NewGuid(), OrderId = order1.Id, ProductVariantId = menTshirtVariants[0].Id,  Quantity = 2 },
      new OrderItem { Id = Guid.NewGuid(), OrderId = order1.Id, ProductVariantId = menCapVariants[0].Id,     Quantity = 1 },
      new OrderItem { Id = Guid.NewGuid(), OrderId = order2.Id, ProductVariantId = womenJeansVariants[0].Id, Quantity = 1 }
    );

    // Wishlists
    var wishlist1 = new Wishlist { Id = Guid.NewGuid(), UserId = customer1.Id };
    var wishlist2 = new Wishlist { Id = Guid.NewGuid(), UserId = customer2.Id };
    context.Wishlists.AddRange(wishlist1, wishlist2);
    context.SaveChanges();

    context.WishlistItems.AddRange(
      new WishlistItem { Id = Guid.NewGuid(), WishlistId = wishlist1.Id, ProductId = menHoodie.Id,   AddedAt = DateTime.UtcNow },
      new WishlistItem { Id = Guid.NewGuid(), WishlistId = wishlist2.Id, ProductId = womenTshirt.Id, AddedAt = DateTime.UtcNow }
    );

    // Carts
    var cart1 = new Cart { Id = Guid.NewGuid(), UserId = customer1.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
    var cart2 = new Cart { Id = Guid.NewGuid(), UserId = customer2.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
    context.Carts.AddRange(cart1, cart2);
    context.SaveChanges();

    context.CartItems.AddRange(
      new CartItem { Id = Guid.NewGuid(), CartId = cart1.Id, ProductVariantId = menHoodieVariants[0].Id,   Quantity = 1 },
      new CartItem { Id = Guid.NewGuid(), CartId = cart2.Id, ProductVariantId = womenTshirtVariants[1].Id, Quantity = 2 }
    );

    // Payments
    context.Payments.AddRange(
      new Payment { Id = Guid.NewGuid(), OrderId = order1.Id, Amount = 76.48, Currency = "USD", Status = PaymentStatus.Succeeded, StripePaymentIntentId = "pi_test_seed_1", CreatedAt = DateTime.UtcNow.AddDays(-5) },
      new Payment { Id = Guid.NewGuid(), OrderId = order2.Id, Amount = 55.99, Currency = "USD", Status = PaymentStatus.Pending,   StripePaymentIntentId = "pi_test_seed_2", CreatedAt = DateTime.UtcNow.AddDays(-1) }
    );

    // Seller review
    context.SellerReviews.Add(new SellerReview
    {
      Id = Guid.NewGuid(),
      SellerProfileId = sellerProfile.Id,
      UserId = customer1.Id,
      Rating = 5,
      Comment = "Excellent seller! Great clothing quality and fast shipping.",
      CreatedAt = DateTime.UtcNow
    });

    await context.SaveChangesAsync();
  }
}
