using Api.Models.Entities.Reviews;

namespace Api.Contracts.Reviews;

public class SellerReviewResponse
{
  public Guid Id { get; init; }
  public Guid SellerProfileId { get; init; }
  public Guid UserId { get; init; }
  public int Rating { get; init; }
  public string? Comment { get; init; }
  public DateTime CreatedAt { get; init; }

  public static SellerReviewResponse From(SellerReview r) => new()
  {
    Id = r.Id,
    SellerProfileId = r.SellerProfileId,
    UserId = r.UserId,
    Rating = r.Rating,
    Comment = r.Comment,
    CreatedAt = r.CreatedAt,
  };
}
