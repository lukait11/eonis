using Api.Contracts.Reviews;
using Api.Data.Interfaces.Catalog;
using Api.Data.Interfaces.Identity;
using Api.Data.Interfaces.Reviews;
using Api.Models.DTO.Reviews;
using Api.Models.Entities.Reviews;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Reviews;

[ApiController]
[Route("api/[controller]")]
public class ProductReviewController(
  IProductReviewRepository productReviewRepository,
  IProductRepository productRepository,
  IApplicationUserRepository applicationUserRepository
) : ControllerBase
{
  [HttpGet("user/{userId:guid}")]
  public async Task<IActionResult> GetByUserId(Guid userId)
  {
    try
    {
      var reviews = await productReviewRepository.GetProductReviewsByUserIdAsync(userId);
      return Ok(reviews.Select(ProductReviewResponse.From));
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
      var reviews = await productReviewRepository.GetProductReviewsByProductIdAsync(productId);
      return Ok(reviews.Select(ProductReviewResponse.From));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("{reviewId:guid}")]
  public async Task<IActionResult> GetById(Guid reviewId)
  {
    try
    {
      var review = await productReviewRepository.GetProductReviewByIdAsync(reviewId);
      if (review == null) return NotFound();
      return Ok(ProductReviewResponse.From(review));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(CreateProductReviewRequest request)
  {
    try
    {
      if (await productRepository.GetProductByIdAsync(request.ProductId) == null)
        return NotFound($"Product with ID {request.ProductId} not found.");
      if (await applicationUserRepository.GetUserByIdAsync(request.UserId) == null)
        return NotFound($"User with ID {request.UserId} not found.");

      var review = new ProductReview
      {
        ProductId = request.ProductId,
        UserId = request.UserId,
        Rating = request.Rating,
        Comment = request.Comment,
      };

      var created = await productReviewRepository.CreateProductReviewAsync(review);
      return CreatedAtAction(nameof(GetById), new { reviewId = created.Id }, ProductReviewResponse.From(created));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("{reviewId:guid}")]
  public async Task<IActionResult> Delete(Guid reviewId)
  {
    try
    {
      var deleted = await productReviewRepository.DeleteProductReviewAsync(reviewId);
      if (!deleted) return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
