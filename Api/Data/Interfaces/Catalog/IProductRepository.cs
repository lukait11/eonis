using Api.Models.Entities.Catalog;

namespace Api.Data.Interfaces.Catalog;

public interface IProductRepository
{
  Task<IEnumerable<Product>> GetProductsAsync();
  Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId);
  Task<Product?> GetProductByIdAsync(Guid productId);
  Task<Product> CreateProductAsync(Product product);
  Task<Product?> UpdateProductAsync(Product product);
  Task<bool> DeleteProductAsync(Guid productId);
}
