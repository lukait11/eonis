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
    try
    {
      var categories = await categoryRepository.GetCategoriesAsync();
      if (categories == null || !categories.Any())
        return NoContent();
      return Ok(categories);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("{categoryId:guid}")]
  public async Task<IActionResult> GetById(Guid categoryId)
  {
    try
    {
      var category = await categoryRepository.GetCategoryByIdAsync(categoryId);
      if (category == null)
        return NoContent();
      return Ok(category);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(Category category)
  {
    try
    {
      if (category.ParentCategoryId.HasValue)
      {
        var parentCategory = await categoryRepository.GetCategoryByIdAsync(category.ParentCategoryId.Value);
        if (parentCategory == null)
          return BadRequest("Parent category does not exist.");
      }
      var createdCategory = await categoryRepository.CreateCategoryAsync(category);
      return CreatedAtAction(nameof(GetById), new { categoryId = createdCategory.Id }, createdCategory);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut]
  public async Task<IActionResult> Update(Category category)
  {
    try
    {
      if (category.Id == Guid.Empty)
        return BadRequest();
      if (category.ParentCategoryId.HasValue)
      {
        var parentCategory = await categoryRepository.GetCategoryByIdAsync(category.ParentCategoryId.Value);
        if (parentCategory == null)
          return BadRequest("Parent category does not exist.");
      }
      var updatedCategory = await categoryRepository.UpdateCategoryAsync(category);
      if (updatedCategory == null)
        return NotFound();
      return Ok(updatedCategory);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("{categoryId:guid}")]
  public async Task<IActionResult> Delete(Guid categoryId)
  {
    try
    {
      var deleted = await categoryRepository.DeleteCategoryAsync(categoryId);
      if (!deleted)
        return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
