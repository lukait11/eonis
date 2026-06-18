using Api.Data.Interfaces.Catalog;
using Api.Models.Entities.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryRepository categoryRepository) : ControllerBase
{
  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    var categories = await categoryRepository.GetCategoriesAsync();
    return Ok(categories);
  }

  [HttpGet("{categoryId:guid}")]
  public async Task<IActionResult> GetById(Guid categoryId)
  {
    var category = await categoryRepository.GetCategoryByIdAsync(categoryId);
    if (category == null)
    {
      return NotFound();
    }
    return Ok(category);
  }

  [HttpPost]
  public async Task<IActionResult> Create(Category category)
  {
    if(category.ParentCategoryId.HasValue)
    {
      var parentCategory = await categoryRepository.GetCategoryByIdAsync(category.ParentCategoryId.Value);
      if (parentCategory == null)
      {
        return BadRequest("Parent category does not exist.");
      }
    }
    var createdCategory = await categoryRepository.CreateCategoryAsync(category);
    return CreatedAtAction(nameof(GetById), new { categoryId = createdCategory.Id }, createdCategory);
  }

  [HttpPut]
  public async Task<IActionResult> Update(Category category)
  {
    if (category.Id == Guid.Empty)
    {
      return BadRequest();
    }
    if(category.ParentCategoryId.HasValue)
    {
      var parentCategory = await categoryRepository.GetCategoryByIdAsync(category.ParentCategoryId.Value);
      if (parentCategory == null)
      {
        return BadRequest("Parent category does not exist.");
      }
    }
    var updatedCategory = await categoryRepository.UpdateCategoryAsync(category);
    if (updatedCategory == null)
    {
      return NotFound();
    }
    return Ok(updatedCategory);
  }

  [HttpDelete("{categoryId:guid}")]
  public async Task<IActionResult> Delete(Guid categoryId)
  {
    var deleted = await categoryRepository.DeleteCategoryAsync(categoryId);
    if (!deleted)
    {
      return NotFound();
    }
    return Ok();
  }
}
