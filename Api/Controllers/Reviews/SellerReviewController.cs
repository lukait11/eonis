using Api.Contracts.Reviews;
using Api.Data.Interfaces.Identity;
using Api.Data.Interfaces.Reviews;
using Api.Models.DTO.Reviews;
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
    try
    {
      var reviews = await sellerReviewRepository.GetSellerReviewsByUserIdAsync(userId);
      return Ok(reviews.Select(SellerReviewResponse.From));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("seller/{sellerId:guid}")]
  public async Task<IActionResult> GetBySellerId(Guid sellerId)
  {
    try
    {
      var reviews = await sellerReviewRepository.GetSellerReviewsBySellerIdAsync(sellerId);
      return Ok(reviews.Select(SellerReviewResponse.From));
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
      var review = await sellerReviewRepository.GetSellerReviewByIdAsync(reviewId);
      if (review == null) return NotFound();
      return Ok(SellerReviewResponse.From(review));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(CreateSellerReviewRequest request)
  {
    try
    {
      if (await sellerProfileRepository.GetSellerProfileByIdAsync(request.SellerProfileId) == null)
        return NotFound($"Seller profile with ID {request.SellerProfileId} not found.");
      if (await applicationUserRepository.GetUserByIdAsync(request.UserId) == null)
        return NotFound($"User with ID {request.UserId} not found.");

      var review = new SellerReview
      {
        SellerProfileId = request.SellerProfileId,
        UserId = request.UserId,
        Rating = request.Rating,
        Comment = request.Comment,
      };

      var created = await sellerReviewRepository.CreateSellerReviewAsync(review);
      return CreatedAtAction(nameof(GetById), new { reviewId = created.Id }, SellerReviewResponse.From(created));
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
      var deleted = await sellerReviewRepository.DeleteSellerReviewAsync(reviewId);
      if (!deleted) return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
