using Api.Models.Entities.Reviews;

namespace Api.Data.Interfaces.Reviews;

public interface IProductReviewRepository
{
  Task<IEnumerable<ProductReview>> GetProductReviewsByUserIdAsync(Guid userId);
  Task<IEnumerable<ProductReview>> GetProductReviewsByProductIdAsync(Guid productId);
  Task<ProductReview?> GetProductReviewByIdAsync(Guid productReviewId);
  Task<ProductReview> CreateProductReviewAsync(ProductReview productReview);
  Task<bool> DeleteProductReviewAsync(Guid productReviewId);
}
