using Api.Context;
using Api.Data.Interfaces;
using Api.Models.Entities.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Catalog;

public class CategoryRepository(DatabaseContext context) : ICategoryRepository
{
  public async Task<Category> CreateCategoryAsync(Category category)
  {
    context.Categories.Add(category);
    await context.SaveChangesAsync();
    return category;
  }

  public async Task<bool> DeleteCategoryAsync(Guid categoryId)
  {
    var category = await GetCategoryByIdAsync(categoryId);
    if (category == null) return false;

    context.Categories.Remove(category);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<IEnumerable<Category>> GetCategoriesAsync()
  {
    return await context.Categories.ToListAsync();
  }

  public async Task<Category?> GetCategoryByIdAsync(Guid categoryId)
  {
    return await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
  }

  public async Task<Category?> UpdateCategoryAsync(Category category)
  {
    context.Categories.Update(category);
    await context.SaveChangesAsync();
    return category;
  }
}
