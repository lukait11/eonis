using Api.Data.Interfaces.Catalog;
using Api.Models.Entities.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
public class ProductVariantController(
  IProductVariantRepository productVariantRepository,
  IProductRepository productRepository
) : ControllerBase
{
  [HttpGet("{productVariantId:guid}")]
  public async Task<IActionResult> GetById(Guid productVariantId)
  {
    try
    {
      var productVariant = await productVariantRepository.GetProductVariantByIdAsync(productVariantId);
      if (productVariant == null)
        return NoContent();
      return Ok(productVariant);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("product/{productId:guid}")]
  public async Task<IActionResult> GetByProductId(Guid productId)
  {
    try
    {
      var productVariants = await productVariantRepository.GetProductVariantsByProductIdAsync(productId);
      if (productVariants == null || !productVariants.Any())
        return NoContent();
      return Ok(productVariants);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(ProductVariant productVariant)
  {
    try
    {
      if (await productRepository.GetProductByIdAsync(productVariant.ProductId) == null)
        return BadRequest("Product does not exist.");
      var createdProductVariant = await productVariantRepository.CreateProductVariantAsync(productVariant);
      return CreatedAtAction(nameof(GetById), new { productVariantId = createdProductVariant.Id }, createdProductVariant);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut]
  public async Task<IActionResult> Update(ProductVariant productVariant)
  {
    try
    {
      if (productVariant.Id == Guid.Empty)
        return BadRequest();
      if (await productRepository.GetProductByIdAsync(productVariant.ProductId) == null)
        return BadRequest("Product does not exist.");
      var updatedProductVariant = await productVariantRepository.UpdateProductVariantAsync(productVariant);
      if (updatedProductVariant == null)
        return NotFound();
      return Ok(updatedProductVariant);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("{productVariantId:guid}")]
  public async Task<IActionResult> Delete(Guid productVariantId)
  {
    try
    {
      var deleted = await productVariantRepository.DeleteProductVariantAsync(productVariantId);
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
