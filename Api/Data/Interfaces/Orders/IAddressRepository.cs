using Api.Models.Entities.Orders;

namespace Api.Data.Interfaces.Orders;

public interface IAddressRepository
{
  Task<IEnumerable<Address>> GetAddressesByUserIdAsync(Guid userId);
  Task<Address?> GetAddressByIdAsync(Guid addressId);
  Task<Address> CreateAddressAsync(Address address);
  Task<Address?> UpdateAddressAsync(Address address);
  Task<bool> DeleteAddressAsync(Guid addressId);
}
