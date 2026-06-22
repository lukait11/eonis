using System.Security.Claims;
using Api.Data.Interfaces.Catalog;
using Api.Data.Interfaces.Identity;
using Api.Models.Entities.Catalog;
using Api.Models.Entities.Identity;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
public class ProductController(
  IProductRepository productRepository,
  IProductImageRepository productImageRepository,
  ICategoryRepository categoryRepository,
  IApplicationUserRepository applicationUserRepository,
  ImageService imageService
) : ControllerBase
{
  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    try
    {
      var products = await productRepository.GetProductsAsync();
      if (products == null || !products.Any())
        return NoContent();
      return Ok(products);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("{productId:guid}")]
  public async Task<IActionResult> GetById(Guid productId)
  {
    try
    {
      var product = await productRepository.GetProductByIdAsync(productId);
      if (product == null)
        return NoContent();
      return Ok(product);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("category/{categoryId:guid}")]
  public async Task<IActionResult> GetByCategoryId(Guid categoryId)
  {
    try
    {
      var products = await productRepository.GetProductsByCategoryIdAsync(categoryId);
      if (products == null || !products.Any())
        return NoContent();
      return Ok(products);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(Product product)
  {
    try
    {
      if (product.CategoryId.HasValue)
      {
        var category = await categoryRepository.GetCategoryByIdAsync(product.CategoryId.Value);
        if (category == null)
          return BadRequest("Category does not exist.");
      }
      var createdProduct = await productRepository.CreateProductAsync(product);
      return CreatedAtAction(nameof(GetById), new { productId = createdProduct.Id }, createdProduct);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut]
  public async Task<IActionResult> Update(Product product)
  {
    try
    {
      if (product.Id == Guid.Empty)
        return BadRequest();
      if (product.CategoryId.HasValue)
      {
        var category = await categoryRepository.GetCategoryByIdAsync(product.CategoryId.Value);
        if (category == null)
          return BadRequest("Category does not exist.");
      }
      var updatedProduct = await productRepository.UpdateProductAsync(product);
      if (updatedProduct == null)
        return NotFound();
      return Ok(updatedProduct);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("{productId:guid}")]
  public async Task<IActionResult> Delete(Guid productId)
  {
    try
    {
      var deleted = await productRepository.DeleteProductAsync(productId);
      if (!deleted)
        return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [Authorize]
  [HttpPost("{productId:guid}/image")]
  [RequestSizeLimit(6 * 1024 * 1024)]
  [RequestFormLimits(MultipartBodyLengthLimit = 6 * 1024 * 1024)]
  public async Task<IActionResult> UploadImage(Guid productId, IFormFile file, CancellationToken ct)
  {
    try
    {
      if (file is null || file.Length == 0)
        return BadRequest("No file provided.");

      if (await productRepository.GetProductByIdAsync(productId) == null)
        return BadRequest("Product does not exist.");

      var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var user = await applicationUserRepository.GetUserByIdAsync(userId);
      if (user is null) return Unauthorized();

      if (user.Role != UserRole.Seller || user.Role != UserRole.Admin)
        return Forbid();

      var product = await productRepository.GetProductByIdAsync(productId);
      if (product == null)
        return NotFound();
      if (product.SellerId != userId)
        return Forbid();

      string newUrl;
      try
      {
        await using var stream = file.OpenReadStream();
        newUrl = await imageService.ProcessAndUploadAsync(stream, file.ContentType, productId, ct);
        ProductImage newImage = new()
        {
          ProductId = productId,
          ImageUrl = newUrl,
          IsPrimary = false
        };

        await productImageRepository.CreateProductImageAsync(newImage);
      }
      catch (ImageValidationException ex)
      {
        return BadRequest(ex.Message);
      }
      return Ok(newUrl);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
