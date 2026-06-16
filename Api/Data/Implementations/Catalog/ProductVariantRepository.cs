using Api.Context;
using Api.Data.Interfaces.Catalog;
using Api.Models.Entities.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Catalog;

public class ProductVariantRepository(DatabaseContext context) : IProductVariantRepository
{
  public async Task<ProductVariant> CreateProductVariantAsync(ProductVariant productVariant)
  {
    context.ProductVariants.Add(productVariant);
    await context.SaveChangesAsync();
    return productVariant;
  }

  public async Task<bool> DeleteProductVariantAsync(Guid productVariantId)
  {
    var productVariant = await GetProductVariantByIdAsync(productVariantId);
    if (productVariant == null) return false;

    context.ProductVariants.Remove(productVariant);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<ProductVariant?> GetProductVariantByIdAsync(Guid productVariantId)
  {
    return await context.ProductVariants.Include(pv => pv.Images).FirstOrDefaultAsync(pv => pv.Id == productVariantId);
  }

  public async Task<IEnumerable<ProductVariant>> GetProductVariantsByProductIdAsync(Guid productId)
  {
    return await context.ProductVariants.Include(pv => pv.Images).Where(pv => pv.ProductId == productId).ToListAsync();
  }

  public async Task<ProductVariant?> UpdateProductVariantAsync(ProductVariant productVariant)
  {
    throw new NotImplementedException();
  }

}
