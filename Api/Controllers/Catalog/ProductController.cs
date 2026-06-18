using Api.Data.Interfaces.Catalog;
using Api.Models.Entities.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
public class ProductController(
  IProductRepository productRepository,
  ICategoryRepository categoryRepository
) : ControllerBase
{
  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    var products = await productRepository.GetProductsAsync();
    return Ok(products);
  }

  [HttpGet("{productId:guid}")]
  public async Task<IActionResult> GetById(Guid productId)
  {
    var product = await productRepository.GetProductByIdAsync(productId);
    if (product == null)
    {
      return NotFound();
    }
    return Ok(product);
  }

  [HttpGet("/category/{categoryId:guid}")]
  public async Task<IActionResult> GetByCategoryId(Guid categoryId)
  {
    var products = await productRepository.GetProductsByCategoryIdAsync(categoryId);
    if (products == null || !products.Any())
    {
      return NotFound();
    }
    return Ok(products);
  }

  [HttpPost]
  public async Task<IActionResult> Create(Product product)
  {
    if(product.CategoryId.HasValue)
    {
      var category = await categoryRepository.GetCategoryByIdAsync(product.CategoryId.Value);
      if (category == null)
      {
        return BadRequest("Category does not exist.");
      }
    }
    var createdProduct = await productRepository.CreateProductAsync(product);
    return CreatedAtAction(nameof(GetById), new { productId = createdProduct.Id }, createdProduct);
  }

  [HttpPut]
  public async Task<IActionResult> Update(Product product)
  {
    if (product.Id == Guid.Empty)
    {
      return BadRequest();
    }
    if(product.CategoryId.HasValue)
    {
      var category = await categoryRepository.GetCategoryByIdAsync(product.CategoryId.Value);
      if (category == null)
      {
        return BadRequest("Category does not exist.");
      }
    }
    var updatedProduct = await productRepository.UpdateProductAsync(product);
    if (updatedProduct == null)
    {
      return NotFound();
    }
    return Ok(updatedProduct);
  }

  [HttpDelete("{productId:guid}")]
  public async Task<IActionResult> Delete(Guid productId)
  {
    var deleted = await productRepository.DeleteProductAsync(productId);
    if (!deleted)
    {
      return NotFound();
    }
    return Ok();
  }
}
