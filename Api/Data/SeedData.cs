using Api.Context;
using Api.Models.Entities.Catalog;
using Api.Models.Entities.Identity;
using Api.Models.Entities.Orders;
using Api.Models.Entities.Payments;
using Api.Models.Entities.Reviews;
using Api.Models.Entities.Shopping;
using Api.Models.Entities.Wishlists;
using Microsoft.AspNetCore.Identity;

namespace Api.Data;

public static class SeedData
{
  public static async Task InitializeAsync(
    DatabaseContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager)
  {
    // Create roles
    var roles = new[] { "Admin", "Seller", "Customer" };
    foreach (var role in roles)
    {
      if (!await roleManager.RoleExistsAsync(role))
      {
        await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
      }
    }

    // Create users
    var seller = new ApplicationUser
    {
      Id = Guid.NewGuid(),
      Role = UserRole.Seller,
      Email = "seller@example.com",
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
      FirstName = "Bob",
      LastName = "Johnson",
      IsActive = true,
      CreatedAt = DateTime.UtcNow
    };

    await userManager.CreateAsync(seller, "Password123!");
    await userManager.CreateAsync(customer1, "Password123!");
    await userManager.CreateAsync(customer2, "Password123!");

    await userManager.AddToRoleAsync(seller, "Seller");
    await userManager.AddToRoleAsync(customer1, "Customer");
    await userManager.AddToRoleAsync(customer2, "Customer");

    // Create seller profile
    var sellerProfile = new SellerProfile
    {
      Id = Guid.NewGuid(),
      UserId = seller.Id,
      StoreName = "StyleHub",
      Description = "Premium clothing and fashion for all"
    };

    context.SellerProfiles.Add(sellerProfile);

    // Create categories
    var menCategory = new Category
    {
      Id = Guid.NewGuid(),
      Name = "Men",
      Description = "Men's clothing and accessories"
    };

    var womenCategory = new Category
    {
      Id = Guid.NewGuid(),
      Name = "Women",
      Description = "Women's clothing and accessories"
    };

    var kidsCategory = new Category
    {
      Id = Guid.NewGuid(),
      Name = "Kids",
      Description = "Children's clothing"
    };

    var menTshirtsCategory = new Category
    {
      Id = Guid.NewGuid(),
      Name = "T-Shirts",
      Description = "Men's t-shirts and casual tops",
      ParentCategoryId = menCategory.Id
    };

    var menCapsCategory = new Category
    {
      Id = Guid.NewGuid(),
      Name = "Caps & Hats",
      Description = "Men's caps and hats",
      ParentCategoryId = menCategory.Id
    };

    var womenTshirtsCategory = new Category
    {
      Id = Guid.NewGuid(),
      Name = "T-Shirts",
      Description = "Women's t-shirts and tops",
      ParentCategoryId = womenCategory.Id
    };

    var womenJeansCategory = new Category
    {
      Id = Guid.NewGuid(),
      Name = "Jeans",
      Description = "Women's jeans and denim",
      ParentCategoryId = womenCategory.Id
    };

    context.Categories.AddRange(
      menCategory, womenCategory, kidsCategory, 
      menTshirtsCategory, menCapsCategory, 
      womenTshirtsCategory, womenJeansCategory);
    context.SaveChanges();

    // Create products
    var menTshirt = new Product
    {
      Id = Guid.NewGuid(),
      SellerId = sellerProfile.Id,
      CategoryId = menTshirtsCategory.Id,
      Name = "Premium Cotton T-Shirt",
      Description = "Comfortable and stylish 100% cotton t-shirt for everyday wear",
      BasePrice = 29.99,
      Discount = 15,
      Material = "100% Cotton",
      Status = "Active",
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    var menHoodie = new Product
    {
      Id = Guid.NewGuid(),
      SellerId = sellerProfile.Id,
      CategoryId = menTshirtsCategory.Id,
      Name = "Casual Hoodie",
      Description = "Warm and cozy hoodie perfect for cold weather",
      BasePrice = 59.99,
      Discount = 20,
      Material = "Cotton Blend",
      Status = "Active",
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    var menCap = new Product
    {
      Id = Guid.NewGuid(),
      SellerId = sellerProfile.Id,
      CategoryId = menCapsCategory.Id,
      Name = "Classic Baseball Cap",
      Description = "Adjustable baseball cap with embroidered logo",
      BasePrice = 24.99,
      Discount = 10,
      Material = "Cotton Canvas",
      Status = "Active",
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    var womenTshirt = new Product
    {
      Id = Guid.NewGuid(),
      SellerId = sellerProfile.Id,
      CategoryId = womenTshirtsCategory.Id,
      Name = "Fitted V-Neck T-Shirt",
      Description = "Elegant fitted t-shirt with v-neckline",
      BasePrice = 34.99,
      Discount = 25,
      Material = "100% Cotton",
      Status = "Active",
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    var womenJeans = new Product
    {
      Id = Guid.NewGuid(),
      SellerId = sellerProfile.Id,
      CategoryId = womenJeansCategory.Id,
      Name = "Slim Fit Denim Jeans",
      Description = "Classic slim fit jeans with perfect stretch",
      BasePrice = 79.99,
      Discount = 30,
      Material = "98% Cotton, 2% Elastane",
      Status = "Active",
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    context.Products.AddRange(menTshirt, menHoodie, menCap, womenTshirt, womenJeans);
    context.SaveChanges();

    // Create product images
    var menTshirtImages = new[]
    {
      new ProductImage
      {
        Id = Guid.NewGuid(),
        ProductId = menTshirt.Id,
        ImageUrl = "https://via.placeholder.com/500?text=Men+T-Shirt+Front",
        IsPrimary = true
      },
      new ProductImage
      {
        Id = Guid.NewGuid(),
        ProductId = menTshirt.Id,
        ImageUrl = "https://via.placeholder.com/500?text=Men+T-Shirt+Back",
        IsPrimary = false
      }
    };

    var menHoodieImages = new[]
    {
      new ProductImage
      {
        Id = Guid.NewGuid(),
        ProductId = menHoodie.Id,
        ImageUrl = "https://via.placeholder.com/500?text=Hoodie+Front",
        IsPrimary = true
      }
    };

    var menCapImages = new[]
    {
      new ProductImage
      {
        Id = Guid.NewGuid(),
        ProductId = menCap.Id,
        ImageUrl = "https://via.placeholder.com/500?text=Baseball+Cap",
        IsPrimary = true
      }
    };

    var womenTshirtImages = new[]
    {
      new ProductImage
      {
        Id = Guid.NewGuid(),
        ProductId = womenTshirt.Id,
        ImageUrl = "https://via.placeholder.com/500?text=Women+V-Neck+Front",
        IsPrimary = true
      }
    };

    var womenJeansImages = new[]
    {
      new ProductImage
      {
        Id = Guid.NewGuid(),
        ProductId = womenJeans.Id,
        ImageUrl = "https://via.placeholder.com/500?text=Women+Jeans+Front",
        IsPrimary = true
      }
    };

    context.ProductImages.AddRange(menTshirtImages);
    context.ProductImages.AddRange(menHoodieImages);
    context.ProductImages.AddRange(menCapImages);
    context.ProductImages.AddRange(womenTshirtImages);
    context.ProductImages.AddRange(womenJeansImages);

    // Create product variants
    var menTshirtVariants = new[]
    {
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = menTshirt.Id,
        Size = "S",
        Color = "Black",
        Quantity = 100
      },
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = menTshirt.Id,
        Size = "M",
        Color = "Black",
        Quantity = 150
      },
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = menTshirt.Id,
        Size = "L",
        Color = "White",
        Quantity = 120
      },
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = menTshirt.Id,
        Size = "XL",
        Color = "Navy",
        Quantity = 80
      }
    };

    var menHoodieVariants = new[]
    {
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = menHoodie.Id,
        Size = "S",
        Color = "Gray",
        Quantity = 50
      },
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = menHoodie.Id,
        Size = "M",
        Color = "Gray",
        Quantity = 70
      },
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = menHoodie.Id,
        Size = "L",
        Color = "Black",
        Quantity = 60
      }
    };

    var menCapVariants = new[]
    {
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = menCap.Id,
        Size = "One Size",
        Color = "Black",
        Quantity = 200
      },
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = menCap.Id,
        Size = "One Size",
        Color = "Navy",
        Quantity = 180
      },
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = menCap.Id,
        Size = "One Size",
        Color = "Red",
        Quantity = 150
      }
    };

    var womenTshirtVariants = new[]
    {
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = womenTshirt.Id,
        Size = "XS",
        Color = "White",
        Quantity = 90
      },
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = womenTshirt.Id,
        Size = "S",
        Color = "Pink",
        Quantity = 110
      },
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = womenTshirt.Id,
        Size = "M",
        Color = "Pink",
        Quantity = 130
      }
    };

    var womenJeansVariants = new[]
    {
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = womenJeans.Id,
        Size = "24",
        Color = "Dark Blue",
        Quantity = 60
      },
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = womenJeans.Id,
        Size = "26",
        Color = "Dark Blue",
        Quantity = 80
      },
      new ProductVariant
      {
        Id = Guid.NewGuid(),
        ProductId = womenJeans.Id,
        Size = "28",
        Color = "Light Blue",
        Quantity = 75
      }
    };

    context.ProductVariants.AddRange(menTshirtVariants);
    context.ProductVariants.AddRange(menHoodieVariants);
    context.ProductVariants.AddRange(menCapVariants);
    context.ProductVariants.AddRange(womenTshirtVariants);
    context.ProductVariants.AddRange(womenJeansVariants);

    // Create product reviews
    var reviews = new[]
    {
      new ProductReview
      {
        Id = Guid.NewGuid(),
        ProductId = menTshirt.Id,
        UserId = customer1.Id,
        Rating = 5,
        Comment = "Great quality cotton! Very comfortable and fits well.",
        CreatedAt = DateTime.UtcNow
      },
      new ProductReview
      {
        Id = Guid.NewGuid(),
        ProductId = menTshirt.Id,
        UserId = customer2.Id,
        Rating = 4,
        Comment = "Good quality, but slightly smaller than expected.",
        CreatedAt = DateTime.UtcNow
      },
      new ProductReview
      {
        Id = Guid.NewGuid(),
        ProductId = menHoodie.Id,
        UserId = customer1.Id,
        Rating = 5,
        Comment = "Perfect hoodie for the season! Warm and cozy.",
        CreatedAt = DateTime.UtcNow
      },
      new ProductReview
      {
        Id = Guid.NewGuid(),
        ProductId = womenJeans.Id,
        UserId = customer2.Id,
        Rating = 4,
        Comment = "Excellent fit and very comfortable!",
        CreatedAt = DateTime.UtcNow
      }
    };

    context.ProductReviews.AddRange(reviews);

    // Create addresses
    var address1 = new Address
    {
      Id = Guid.NewGuid(),
      UserId = customer1.Id,
      Street = "123 Main St",
      City = "New York",
      Country = "USA",
      PostalCode = 10001
    };

    var address2 = new Address
    {
      Id = Guid.NewGuid(),
      UserId = customer2.Id,
      Street = "456 Oak Ave",
      City = "Los Angeles",
      Country = "USA",
      PostalCode = 90001
    };

    context.Addresses.AddRange(address1, address2);

    // Create orders
    var order1 = new Order
    {
      Id = Guid.NewGuid(),
      UserId = customer1.Id,
      AddressId = address1.Id,
      Status = OrderStatus.Delivered,
      CreatedAt = DateTime.UtcNow.AddDays(-5),
      BaseAmount = 89.98,
      Discount = 13.50
    };

    var order2 = new Order
    {
      Id = Guid.NewGuid(),
      UserId = customer2.Id,
      AddressId = address2.Id,
      Status = OrderStatus.Pending,
      CreatedAt = DateTime.UtcNow.AddDays(-1),
      BaseAmount = 79.99,
      Discount = 24.00
    };

    context.Orders.AddRange(order1, order2);

    // Create order items
    var orderItems = new[]
    {
      new OrderItem
      {
        Id = Guid.NewGuid(),
        OrderId = order1.Id,
        ProductVariantId = menTshirtVariants[0].Id,
        Quantity = 2
      },
      new OrderItem
      {
        Id = Guid.NewGuid(),
        OrderId = order1.Id,
        ProductVariantId = menCapVariants[0].Id,
        Quantity = 1
      },
      new OrderItem
      {
        Id = Guid.NewGuid(),
        OrderId = order2.Id,
        ProductVariantId = womenJeansVariants[0].Id,
        Quantity = 1
      }
    };

    context.OrderItems.AddRange(orderItems);

    // Create wishlists
    var wishlist1 = new Wishlist
    {
      Id = Guid.NewGuid(),
      UserId = customer1.Id
    };

    var wishlist2 = new Wishlist
    {
      Id = Guid.NewGuid(),
      UserId = customer2.Id
    };

    context.Wishlists.AddRange(wishlist1, wishlist2);
    context.SaveChanges();

    // Create wishlist items
    var wishlistItems = new[]
    {
      new WishlistItem
      {
        Id = Guid.NewGuid(),
        WishlistId = wishlist1.Id,
        ProductId = menHoodie.Id,
        AddedAt = DateTime.UtcNow
      },
      new WishlistItem
      {
        Id = Guid.NewGuid(),
        WishlistId = wishlist2.Id,
        ProductId = womenTshirt.Id,
        AddedAt = DateTime.UtcNow
      }
    };

    context.WishlistItems.AddRange(wishlistItems);

    // Create shopping carts
    var cart1 = new Cart
    {
      Id = Guid.NewGuid(),
      UserId = customer1.Id,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    var cart2 = new Cart
    {
      Id = Guid.NewGuid(),
      UserId = customer2.Id,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    context.Carts.AddRange(cart1, cart2);
    context.SaveChanges();

    // Create cart items
    var cartItems = new[]
    {
      new CartItem
      {
        Id = Guid.NewGuid(),
        CartId = cart1.Id,
        ProductVariantId = menHoodieVariants[0].Id,
        Quantity = 1
      },
      new CartItem
      {
        Id = Guid.NewGuid(),
        CartId = cart2.Id,
        ProductVariantId = womenTshirtVariants[1].Id,
        Quantity = 2
      }
    };

    context.CartItems.AddRange(cartItems);

    // Create payments
    var payment1 = new Payment
    {
      Id = Guid.NewGuid(),
      OrderId = order1.Id,
      Amount = 76.48,
      Currency = "USD",
      Status = PaymentStatus.Succeeded,
      StripePaymentIntentId = Guid.NewGuid(),
      CreatedAt = DateTime.UtcNow.AddDays(-5)
    };

    var payment2 = new Payment
    {
      Id = Guid.NewGuid(),
      OrderId = order2.Id,
      Amount = 55.99,
      Currency = "USD",
      Status = PaymentStatus.Pending,
      StripePaymentIntentId = Guid.NewGuid(),
      CreatedAt = DateTime.UtcNow.AddDays(-1)
    };

    context.Payments.AddRange(payment1, payment2);

    // Create seller reviews
    var sellerReview = new SellerReview
    {
      Id = Guid.NewGuid(),
      SellerProfileId = sellerProfile.Id,
      UserId = customer1.Id,
      Rating = 5,
      Comment = "Excellent seller! Great clothing quality and fast shipping.",
      CreatedAt = DateTime.UtcNow
    };

    context.SellerReviews.Add(sellerReview);

    await context.SaveChangesAsync();
  }
}
