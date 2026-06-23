using Api.Contracts.Catalog;
using Api.Data.Interfaces.Catalog;
using Api.Models.DTO.Catalog;
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
      var variant = await productVariantRepository.GetProductVariantByIdAsync(productVariantId);
      if (variant == null) return NotFound();
      return Ok(ProductVariantResponse.From(variant));
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
      var variants = await productVariantRepository.GetProductVariantsByProductIdAsync(productId);
      return Ok(variants.Select(ProductVariantResponse.From));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(CreateProductVariantRequest request)
  {
    try
    {
      if (await productRepository.GetProductByIdAsync(request.ProductId) == null)
        return BadRequest("Product does not exist.");

      var variant = new ProductVariant
      {
        ProductId = request.ProductId,
        Size = request.Size,
        Color = request.Color,
        Quantity = request.Quantity,
      };

      var created = await productVariantRepository.CreateProductVariantAsync(variant);
      return CreatedAtAction(nameof(GetById), new { productVariantId = created.Id }, ProductVariantResponse.From(created));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut("{productVariantId:guid}")]
  public async Task<IActionResult> Update(Guid productVariantId, UpdateProductVariantRequest request)
  {
    try
    {
      if (await productRepository.GetProductByIdAsync(request.ProductId) == null)
        return BadRequest("Product does not exist.");

      var existing = await productVariantRepository.GetProductVariantByIdAsync(productVariantId);
      if (existing == null) return NotFound();

      existing.Size = request.Size;
      existing.Color = request.Color;
      existing.Quantity = request.Quantity;

      var updated = await productVariantRepository.UpdateProductVariantAsync(existing);
      return Ok(ProductVariantResponse.From(updated!));
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
      if (!deleted) return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
