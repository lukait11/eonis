using Api.Models.Entities.Reviews;

namespace Api.Contracts.Reviews;

public class ProductReviewResponse
{
  public Guid Id { get; init; }
  public Guid ProductId { get; init; }
  public Guid UserId { get; init; }
  public int Rating { get; init; }
  public string? Comment { get; init; }
  public DateTime CreatedAt { get; init; }

  public static ProductReviewResponse From(ProductReview r) => new()
  {
    Id = r.Id,
    ProductId = r.ProductId,
    UserId = r.UserId,
    Rating = r.Rating,
    Comment = r.Comment,
    CreatedAt = r.CreatedAt,
  };
}
