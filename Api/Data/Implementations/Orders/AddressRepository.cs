using Api.Context;
using Api.Data.Interfaces.Orders;
using Api.Models.Entities.Orders;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Orders;

public class AddressRepository(DatabaseContext context) : IAddressRepository
{
  public async Task<Address> CreateAddressAsync(Address address)
  {
    context.Addresses.Add(address);
    await context.SaveChangesAsync();
    return address;
  }

  public async Task<bool> DeleteAddressAsync(Guid addressId)
  {
    var address = await GetAddressByIdAsync(addressId);
    if (address == null) return false;

    context.Addresses.Remove(address);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<Address?> GetAddressByIdAsync(Guid addressId)
  {
    return await context.Addresses.FirstOrDefaultAsync(a => a.Id == addressId);
  }

  public async Task<IEnumerable<Address>> GetAddressesByUserIdAsync(Guid userId)
  {
    return await context.Addresses.Where(a => a.UserId == userId).ToListAsync();
  }

  public async Task<Address?> UpdateAddressAsync(Address address)
  {
    context.Addresses.Update(address);
    await context.SaveChangesAsync();
    return address;
  }
}