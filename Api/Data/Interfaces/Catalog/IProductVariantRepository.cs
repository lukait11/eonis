using Api.Models.Entities.Catalog;

namespace Api.Data.Interfaces.Catalog;

public interface IProductVariantRepository
{
  Task<IEnumerable<ProductVariant>> GetProductVariantsByProductIdAsync(Guid productId);
  Task<ProductVariant?> GetProductVariantByIdAsync(Guid productVariantId);
  Task<ProductVariant> CreateProductVariantAsync(ProductVariant productVariant);
  Task<ProductVariant?> UpdateProductVariantAsync(ProductVariant productVariant);
  Task<bool> DeleteProductVariantAsync(Guid productVariantId);
}
