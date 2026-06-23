using Api.Context;
using Api.Data.Interfaces.Catalog;
using Api.Models;
using Api.Models.Entities.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Catalog;

public class ProductRepository(DatabaseContext context) : IProductRepository
{
  public async Task<PagedResult<Product>> GetProductsPagedAsync(int page, int pageSize, string? search, Guid? categoryId, string? sort)
  {
    var query = context.Products
      .Include(p => p.Images.Where(i => i.IsPrimary == true))
      .Include(p => p.Variants)
      .AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
      query = query.Where(p => p.Name != null && p.Name.ToLower().Contains(search.ToLower()));

    if (categoryId.HasValue)
      query = query.Where(p => p.CategoryId == categoryId);

    query = sort switch
    {
      "price-asc"  => query.OrderBy(p => p.BasePrice * (1 - p.Discount / 100.0)),
      "price-desc" => query.OrderByDescending(p => p.BasePrice * (1 - p.Discount / 100.0)),
      "newest"     => query.OrderByDescending(p => p.CreatedAt),
      _            => query.OrderByDescending(p => p.CreatedAt),
    };

    var totalCount = await query.CountAsync();
    var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

    return new PagedResult<Product>(items, totalCount, page, pageSize);
  }


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
      .Include(p => p.Category)
      .Include(p => p.Seller).ThenInclude(s => s!.User)
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
    var existing = await context.Products.FindAsync(product.Id);
    if (existing == null) return null;

    existing.Name = product.Name;
    existing.Description = product.Description;
    existing.BasePrice = product.BasePrice;
    existing.Discount = product.Discount;
    existing.Material = product.Material;
    existing.Status = product.Status;
    existing.CategoryId = product.CategoryId;
    existing.UpdatedAt = DateTime.UtcNow;

    await context.SaveChangesAsync();
    return existing;
  }
}
