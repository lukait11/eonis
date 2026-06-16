using Api.Models.Entities.Reviews;

namespace Api.Data.Interfaces.Reviews;

public interface ISellerReviewRepository
{
  Task<IEnumerable<SellerReview>> GetSellerReviewsByUserIdAsync(Guid userId);
  Task<IEnumerable<SellerReview>> GetSellerReviewsBySellerIdAsync(Guid sellerId);
  Task<SellerReview?> GetSellerReviewByIdAsync(Guid sellerReviewId);
  Task<SellerReview> CreateSellerReviewAsync(SellerReview sellerReview);
  Task<bool> DeleteSellerReviewAsync(Guid sellerReviewId);
}
