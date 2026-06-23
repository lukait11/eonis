using Api.Context;
using Api.Data.Interfaces.Catalog;
using Api.Models.Entities.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Catalog;

public class ProductImageRepository(DatabaseContext context) : IProductImageRepository
{
  public async Task<ProductImage> CreateProductImageAsync(ProductImage productImage)
  {
    context.ProductImages.Add(productImage);
    await context.SaveChangesAsync();
    return productImage;
  }

  public async Task<bool> DeleteProductImageAsync(Guid productImageId)
  {
    var productImage = await GetProductImageByIdAsync(productImageId);
    if (productImage == null) return false;

    context.ProductImages.Remove(productImage);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<ProductImage?> GetProductImageByIdAsync(Guid productImageId)
  {
    return await context.ProductImages.FirstOrDefaultAsync(pi => pi.Id == productImageId);
  }

  public async Task<IEnumerable<ProductImage>> GetProductImagesByProductIdAsync(Guid productId)
  {
    return await context.ProductImages.Where(pi => pi.ProductId == productId).ToListAsync();
  }

  public async Task<ProductImage?> UpdateProductImageAsync(ProductImage productImage)
  {
    var existingProductImage = await GetProductImageByIdAsync(productImage.Id);
    if (existingProductImage == null) return null;

    context.Entry(existingProductImage).CurrentValues.SetValues(productImage);
    await context.SaveChangesAsync();
    return existingProductImage;
  }

  public async Task<bool> SetPrimaryImageAsync(Guid imageId, Guid productId)
  {
    var images = await context.ProductImages
      .Where(pi => pi.ProductId == productId)
      .ToListAsync();

    var target = images.FirstOrDefault(i => i.Id == imageId);
    if (target == null) return false;

    foreach (var img in images)
      img.IsPrimary = img.Id == imageId;

    await context.SaveChangesAsync();
    return true;
  }
}