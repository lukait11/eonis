using Api.Models.Entities.Catalog;

namespace Api.Data.Interfaces.Catalog;

public interface IProductImageRepository
{
  Task<IEnumerable<ProductImage>> GetProductImagesByProductIdAsync(Guid productId);
  Task<ProductImage?> GetProductImageByIdAsync(Guid productImageId);
  Task<ProductImage> CreateProductImageAsync(ProductImage productImage);
  Task<ProductImage?> UpdateProductImageAsync(ProductImage productImage);
  Task<bool> DeleteProductImageAsync(Guid productImageId);
}
