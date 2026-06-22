using Api.Data.Interfaces.Identity;
using Api.Data.Interfaces.Reviews;
using Api.Models.Entities.Reviews;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Reviews;

[ApiController]
[Route("api/[controller]")]
public class SellerReviewController(
  ISellerReviewRepository sellerReviewRepository,
  ISellerProfileRepository sellerProfileRepository,
  IApplicationUserRepository applicationUserRepository
) : ControllerBase
{
  [HttpGet("user/{userId:guid}")]
  public async Task<IActionResult> GetByUserId(Guid userId)
  {
    var reviews = await sellerReviewRepository.GetSellerReviewsByUserIdAsync(userId);
    if (reviews == null || !reviews.Any())
    {
      return NoContent();
    }
    return Ok(reviews);
  }

  [HttpGet("seller/{sellerId:guid}")]
  public async Task<IActionResult> GetBySellerId(Guid sellerId)
  {
    var reviews = await sellerReviewRepository.GetSellerReviewsBySellerIdAsync(sellerId);
    if (reviews == null || !reviews.Any())
    {
      return NoContent();
    }
    return Ok(reviews);
  }

  [HttpGet("{reviewId:guid}")]
  public async Task<IActionResult> GetById(Guid reviewId)
  {
    var review = await sellerReviewRepository.GetSellerReviewByIdAsync(reviewId);
    if (review == null)
    {
      return NoContent();
    }
    return Ok(review);
  }

  [HttpPost]
  public async Task<IActionResult> Create(SellerReview sellerReview)
  {
    if (await sellerProfileRepository.GetSellerProfileByIdAsync(sellerReview.SellerProfileId) == null)
    {
      return NotFound($"Seller profile with ID {sellerReview.SellerProfileId} not found.");
    }
    if (await applicationUserRepository.GetUserByIdAsync(sellerReview.UserId) == null)
    {
      return NotFound($"User with ID {sellerReview.UserId} not found.");
    }
    var createdReview = await sellerReviewRepository.CreateSellerReviewAsync(sellerReview);
    return CreatedAtAction(nameof(GetById), new { reviewId = createdReview.Id }, createdReview);
  }

  [HttpDelete("{reviewId:guid}")]
  public async Task<IActionResult> Delete(Guid reviewId)
  {
    var deleted = await sellerReviewRepository.DeleteSellerReviewAsync(reviewId);
    if (!deleted)
    {
      return NotFound();
    }
    return Ok();
  }
}
