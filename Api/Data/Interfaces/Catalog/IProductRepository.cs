using Api.Models;
using Api.Models.Entities.Catalog;

namespace Api.Data.Interfaces.Catalog;

public interface IProductRepository
{
  Task<PagedResult<Product>> GetProductsPagedAsync(int page, int pageSize, string? search, Guid? categoryId, string? sort);
  Task<IEnumerable<Product>> GetProductsAsync();
  Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(Guid categoryId);
  Task<Product?> GetProductByIdAsync(Guid productId);
  Task<Product> CreateProductAsync(Product product);
  Task<Product?> UpdateProductAsync(Product product);
  Task<bool> DeleteProductAsync(Guid productId);
}
