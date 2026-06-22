using Api.Data.Interfaces.Catalog;
using Api.Data.Interfaces.Identity;
using Api.Models.Entities.Catalog;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
public class ProductController(
  IProductRepository productRepository,
  ICategoryRepository categoryRepository,
  IApplicationUserRepository applicationUserRepository,
  IStorageService storageService,
  ImageService imageService
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

  [HttpGet("category/{categoryId:guid}")]
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

  //[Authorize]
  [HttpPost("{productId:guid}/image")]
  [RequestSizeLimit(6 * 1024 * 1024)]
  [RequestFormLimits(MultipartBodyLengthLimit = 6 * 1024 * 1024)]
  public async Task<IActionResult> UploadImage(Guid productId, IFormFile file, CancellationToken ct)
  {
    if (file is null || file.Length == 0)
      return BadRequest("No file provided.");

    var userId = Guid.Parse(User.FindFirst("sub")!.Value);
    var user = await applicationUserRepository.GetUserByIdAsync(userId);
    if (user is null) return Unauthorized();
  
    string newUrl;
    try
    {
      await using var stream = file.OpenReadStream();
      newUrl = await imageService.ProcessAndUploadAsync(stream, file.ContentType, productId, ct);
    }
    catch (ImageValidationException ex)
    {
      return BadRequest(ex.Message);
    }

    var oldUrl = user.ProfilePictureUrl;
    user.ProfilePictureUrl = newUrl;
    await applicationUserRepository.UpdateUserAsync(user);

    if (oldUrl is not null)
    {
      var oldKey = ExtractStorageKey(oldUrl);
      if (oldKey is not null)
        await storageService.DeleteAsync(oldKey, ct);
    }

    return Ok(newUrl);
  }

  /// <summary>Extracts the storage key (path after bucket) from a full Garage URL.</summary>
  private static string? ExtractStorageKey(string url)
  {
    // URL format: http://endpoint/bucket/key/path...
    // We need everything after /bucket/
    try
    {
      var uri = new Uri(url);
      var segments = uri.AbsolutePath.TrimStart('/').Split('/', 2);
      return segments.Length == 2 ? segments[1] : null;
    }
    catch
    {
      return null;
    }
  }
}
