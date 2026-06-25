using Api.Contracts.Catalog;
using Api.Data.Interfaces.Catalog;
using Api.Models.DTO.Catalog;
using Api.Models.Entities.Catalog;
using Microsoft.AspNetCore.Authorization;
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
      return Ok(categories.Select(CategoryResponse.From));
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
      if (category == null) return NotFound();
      return Ok(CategoryResponse.From(category));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPost]
  public async Task<IActionResult> Create(CreateCategoryRequest request)
  {
    try
    {
      var category = new Category
      {
        Name = request.Name,
        Description = request.Description,
      };

      var created = await categoryRepository.CreateCategoryAsync(category);
      return CreatedAtAction(nameof(GetById), new { categoryId = created.Id }, CategoryResponse.From(created));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPut("{categoryId:guid}")]
  public async Task<IActionResult> Update(Guid categoryId, UpdateCategoryRequest request)
  {
    try
    {
      var existing = await categoryRepository.GetCategoryByIdAsync(categoryId);
      if (existing == null) return NotFound();

      existing.Name = request.Name;
      existing.Description = request.Description;

      var updated = await categoryRepository.UpdateCategoryAsync(existing);
      return Ok(CategoryResponse.From(updated!));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpDelete("{categoryId:guid}")]
  public async Task<IActionResult> Delete(Guid categoryId)
  {
    try
    {
      var deleted = await categoryRepository.DeleteCategoryAsync(categoryId);
      if (!deleted) return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
