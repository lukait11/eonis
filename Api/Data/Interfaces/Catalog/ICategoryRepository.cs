using Api.Models.Entities.Catalog;

namespace Api.Data.Interfaces;

public interface ICategoryRepository
{
  Task<IEnumerable<Category>> GetCategoriesAsync();
  Task<Category?> GetCategoryByIdAsync(Guid categoryId);
  Task<Category> CreateCategoryAsync(Category category);
  Task<Category?> UpdateCategoryAsync(Category category);
  Task<bool> DeleteCategoryAsync(Guid categoryId);
}
