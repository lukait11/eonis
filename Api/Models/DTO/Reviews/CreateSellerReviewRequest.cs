namespace Api.Models.DTO.Reviews;

public class CreateSellerReviewRequest
{
  public Guid SellerProfileId { get; set; }
  public Guid UserId { get; set; }
  public int Rating { get; set; }
  public string? Comment { get; set; }
}
