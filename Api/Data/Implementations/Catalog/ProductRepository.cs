using Api.Context;
using Api.Data.Interfaces.Catalog;
using Api.Models.Entities.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Catalog;

public class ProductRepository(DatabaseContext context) : IProductRepository
{
  public async Task<Product> CreateProductAsync(Product product)
  {
    product.CreatedAt = DateTime.UtcNow;
    product.UpdatedAt = DateTime.UtcNow;
    context.Products.Add(product);
    await context.SaveChangesAsync();
    return product;
  }

  public async Task<bool> DeleteProductAsync(Guid productId)
  {
    var product = await GetProductByIdAsync(productId);
    if (product == null) return false;

    context.Products.Remove(product);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<Product?> GetProductByIdAsync(Guid productId)
  {
    return await context.Products
      .Include(p => p.Images)
      .Include(p => p.Variants)
      .Include(p => p.Reviews)
      .FirstOrDefaultAsync(p => p.Id == productId);
  }

  public async Task<IEnumerable<Product>> GetProductsAsync()
  {
    return await context.Products
      .Include(p => p.Images.Where(i => i.IsPrimary == true))
      .Include(p => p.Variants)
      .ToListAsync();
  }

  public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId)
  {
    return await context.Products
      .Include(p => p.Images.Where(i => i.IsPrimary == true))
      .Include(p => p.Variants)
      .Where(p => p.CategoryId == categoryId)
      .ToListAsync();
  }

  public async Task<Product?> UpdateProductAsync(Product product)
  {
    product.UpdatedAt = DateTime.UtcNow;
    context.Products.Update(product);
    await context.SaveChangesAsync();
    return product;
  }
}
