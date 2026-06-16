using Api.Models.Entities.Identity;

namespace Api.Data.Interfaces.Identity;

public interface ISellerProfileRepository
{
  Task<IEnumerable<SellerProfile>> GetSellerProfilesAsync();
  Task<SellerProfile?> GetSellerProfileByIdAsync(Guid sellerProfileId);
  Task<SellerProfile> CreateSellerProfileAsync(SellerProfile sellerProfile);
  Task<SellerProfile?> UpdateSellerProfileAsync(SellerProfile sellerProfile);
  Task<bool> DeleteSellerProfileAsync(Guid sellerProfileId);
}
