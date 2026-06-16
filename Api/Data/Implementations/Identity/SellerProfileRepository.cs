using Api.Context;
using Api.Data.Interfaces.Identity;
using Api.Models.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Identity;

public class SellerProfileRepository(DatabaseContext context) : ISellerProfileRepository
{
  public async Task<SellerProfile> CreateSellerProfileAsync(SellerProfile sellerProfile)
  {
    sellerProfile.CreatedAt = DateTime.UtcNow;
    sellerProfile.UpdatedAt = DateTime.UtcNow;
    context.SellerProfiles.Add(sellerProfile);
    await context.SaveChangesAsync();
    return sellerProfile;
  }

  public async Task<bool> DeleteSellerProfileAsync(Guid sellerProfileId)
  {
    var sellerProfile = await GetSellerProfileByIdAsync(sellerProfileId);
    if (sellerProfile == null) return false;

    context.SellerProfiles.Remove(sellerProfile);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<SellerProfile?> GetSellerProfileByIdAsync(Guid sellerProfileId)
  {
    return await context.SellerProfiles.FirstOrDefaultAsync(sp => sp.Id == sellerProfileId);
  }

  public async Task<IEnumerable<SellerProfile>> GetSellerProfilesAsync()
  {
    return await context.SellerProfiles.ToListAsync();
  }

  public async Task<SellerProfile?> UpdateSellerProfileAsync(SellerProfile sellerProfile)
  {
    sellerProfile.UpdatedAt = DateTime.UtcNow;
    context.SellerProfiles.Update(sellerProfile);
    await context.SaveChangesAsync();
    return sellerProfile;
  }
}
