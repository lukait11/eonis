using Api.Data.Interfaces.Catalog;
using Api.Data.Interfaces.Identity;
using Api.Data.Interfaces.Reviews;
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
      if (reviews == null || !reviews.Any())
        return NoContent();
      return Ok(reviews);
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
      if (reviews == null || !reviews.Any())
        return NoContent();
      return Ok(reviews);
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
      if (review == null)
        return NotFound();
      return Ok(review);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(ProductReview productReview)
  {
    try
    {
      if (await productRepository.GetProductByIdAsync(productReview.ProductId) == null)
        return NotFound($"Product with ID {productReview.ProductId} not found.");
      if (await applicationUserRepository.GetUserByIdAsync(productReview.UserId) == null)
        return NotFound($"User with ID {productReview.UserId} not found.");
      var createdReview = await productReviewRepository.CreateProductReviewAsync(productReview);
      return CreatedAtAction(nameof(GetById), new { reviewId = createdReview.Id }, createdReview);
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
