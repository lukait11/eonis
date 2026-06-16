using Api.Context;
using Api.Data.Interfaces.Reviews;
using Api.Models.Entities.Reviews;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Reviews;

public class ProductReviewRepository(DatabaseContext context) : IProductReviewRepository
{
  public async Task<ProductReview> CreateProductReviewAsync(ProductReview productReview)
  {
    productReview.CreatedAt = DateTime.UtcNow;
    context.ProductReviews.Add(productReview);
    await context.SaveChangesAsync();
    return productReview;
  }

  public async Task<bool> DeleteProductReviewAsync(Guid productReviewId)
  {
    var productReview = await GetProductReviewByIdAsync(productReviewId);
    if (productReview == null) return false;

    context.ProductReviews.Remove(productReview);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<ProductReview?> GetProductReviewByIdAsync(Guid productReviewId)
  {
    return await context.ProductReviews.FirstOrDefaultAsync(pr => pr.Id == productReviewId);
  }

  public async Task<IEnumerable<ProductReview>> GetProductReviewsByProductIdAsync(Guid productId)
  {
    return await context.ProductReviews.Where(pr => pr.ProductId == productId).ToListAsync();
  }

  public async Task<IEnumerable<ProductReview>> GetProductReviewsByUserIdAsync(Guid userId)
  {
    return await context.ProductReviews.Where(pr => pr.UserId == userId).ToListAsync();
  }

  public async Task<ProductReview?> UpdateProductReviewAsync(ProductReview productReview)
  {
    context.ProductReviews.Update(productReview);
    await context.SaveChangesAsync();
    return productReview;
  }
}