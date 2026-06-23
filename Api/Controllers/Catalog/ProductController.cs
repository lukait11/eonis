using System.Security.Claims;
using Api.Contracts.Catalog;
using Api.Data.Interfaces.Catalog;
using Api.Data.Interfaces.Identity;
using Api.Models;
using Api.Models.DTO.Catalog;
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
  ISellerProfileRepository sellerProfileRepository,
  ImageService imageService
) : ControllerBase
{
  [HttpGet]
  public async Task<IActionResult> GetAll(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 12,
    [FromQuery] string? search = null,
    [FromQuery] List<Guid>? categoryIds = null,
    [FromQuery] string? sort = null)
  {
    try
    {
      var result = await productRepository.GetProductsPagedAsync(page, pageSize, search, categoryIds, sort);
      var response = new PagedResult<ProductResponse>(
        result.Items.Select(ProductResponse.From),
        result.TotalCount,
        result.Page,
        result.PageSize);
      return Ok(response);
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
      if (product == null) return NotFound();
      return Ok(ProductResponse.From(product));
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
      return Ok(products.Select(ProductResponse.From));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(CreateProductRequest request)
  {
    try
    {
      var categories = new List<Category>();
      foreach (var catId in request.CategoryIds)
      {
        var cat = await categoryRepository.GetCategoryByIdAsync(catId);
        if (cat == null) return BadRequest($"Category {catId} does not exist.");
        categories.Add(cat);
      }

      var product = new Product
      {
        SellerId = request.SellerId,
        Name = request.Name,
        Description = request.Description,
        BasePrice = request.BasePrice,
        Discount = request.Discount,
        Material = request.Material,
        Status = request.Status,
        Categories = categories,
      };

      var created = await productRepository.CreateProductAsync(product);
      return CreatedAtAction(nameof(GetById), new { productId = created.Id }, ProductResponse.From(created));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut("{productId:guid}")]
  public async Task<IActionResult> Update(Guid productId, UpdateProductRequest request)
  {
    try
    {
      var existing = await productRepository.GetProductByIdAsync(productId);
      if (existing == null) return NotFound();

      existing.Name = request.Name;
      existing.Description = request.Description;
      existing.BasePrice = request.BasePrice;
      existing.Discount = request.Discount;
      existing.Material = request.Material;
      existing.Status = request.Status;

      existing.Categories.Clear();
      foreach (var catId in request.CategoryIds)
      {
        var cat = await categoryRepository.GetCategoryByIdAsync(catId);
        if (cat == null) return BadRequest($"Category {catId} does not exist.");
        existing.Categories.Add(cat);
      }

      var updated = await productRepository.UpdateProductAsync(existing);
      return Ok(ProductResponse.From(updated!));
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
      if (!deleted) return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("{productId:guid}/images")]
  public async Task<IActionResult> GetImages(Guid productId)
  {
    try
    {
      var images = await productImageRepository.GetProductImagesByProductIdAsync(productId);
      return Ok(images.Select(ProductImageResponse.From));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [Authorize]
  [HttpPatch("{productId:guid}/image/{imageId:guid}/primary")]
  public async Task<IActionResult> SetPrimaryImage(Guid productId, Guid imageId)
  {
    try
    {
      var product = await productRepository.GetProductByIdAsync(productId);
      if (product == null) return NotFound("Product not found.");

      var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var sellerProfile = await sellerProfileRepository.GetSellerProfileByIdAsync(product.SellerId);
      if (sellerProfile == null || sellerProfile.UserId != userId) return Forbid();

      var success = await productImageRepository.SetPrimaryImageAsync(imageId, productId);
      if (!success) return NotFound("Image not found.");

      return NoContent();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [Authorize]
  [HttpDelete("{productId:guid}/image/{imageId:guid}")]
  public async Task<IActionResult> DeleteImage(Guid productId, Guid imageId)
  {
    try
    {
      var product = await productRepository.GetProductByIdAsync(productId);
      if (product == null) return NotFound("Product not found.");

      var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var sellerProfile = await sellerProfileRepository.GetSellerProfileByIdAsync(product.SellerId);
      if (sellerProfile == null || sellerProfile.UserId != userId) return Forbid();

      var image = await productImageRepository.GetProductImageByIdAsync(imageId);
      if (image == null || image.ProductId != productId) return NotFound("Image not found.");

      await productImageRepository.DeleteProductImageAsync(imageId);
      return NoContent();
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

      var product = await productRepository.GetProductByIdAsync(productId);
      if (product == null) return NotFound("Product does not exist.");

      var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var user = await applicationUserRepository.GetUserByIdAsync(userId);
      if (user is null) return Unauthorized();

      if (user.Role != UserRole.Seller && user.Role != UserRole.Admin)
        return Forbid();

      var sellerProfile = await sellerProfileRepository.GetSellerProfileByIdAsync(product.SellerId);
      if (sellerProfile == null || sellerProfile.UserId != userId)
        return Forbid();

      string proxyUrl;
      try
      {
        await using var stream = file.OpenReadStream();
        var key = await imageService.ProcessAndUploadAsync(stream, file.ContentType, productId, ct);
        proxyUrl = $"{Request.Scheme}://{Request.Host}/api/image/{key}";
        var newImage = new ProductImage
        {
          ProductId = productId,
          ImageUrl = proxyUrl,
          IsPrimary = !product.Images.Any()
        };
        await productImageRepository.CreateProductImageAsync(newImage);
      }
      catch (ImageValidationException ex)
      {
        return BadRequest(ex.Message);
      }

      return Ok(proxyUrl);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
