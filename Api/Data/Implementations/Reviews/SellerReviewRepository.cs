using Api.Context;
using Api.Data.Interfaces.Reviews;
using Api.Models.Entities.Reviews;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Reviews;

public class SellerReviewRepository(DatabaseContext context) : ISellerReviewRepository
{
  public async Task<SellerReview> CreateSellerReviewAsync(SellerReview sellerReview)
  {
    sellerReview.CreatedAt = DateTime.UtcNow;
    context.SellerReviews.Add(sellerReview);
    await context.SaveChangesAsync();
    return sellerReview;
  }

  public async Task<bool> DeleteSellerReviewAsync(Guid sellerReviewId)
  {
    var sellerReview = await GetSellerReviewByIdAsync(sellerReviewId);
    if (sellerReview == null) return false;

    context.SellerReviews.Remove(sellerReview);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<SellerReview?> GetSellerReviewByIdAsync(Guid sellerReviewId)
  {
    return await context.SellerReviews.FirstOrDefaultAsync(sr => sr.Id == sellerReviewId);
  }

  public async Task<IEnumerable<SellerReview>> GetSellerReviewsBySellerIdAsync(Guid sellerId)
  {
    return await context.SellerReviews.Where(sr => sr.SellerProfileId == sellerId).ToListAsync();
  }

  public async Task<IEnumerable<SellerReview>> GetSellerReviewsByUserIdAsync(Guid userId)
  {
    return await context.SellerReviews.Where(sr => sr.UserId == userId).ToListAsync();
  }

}
