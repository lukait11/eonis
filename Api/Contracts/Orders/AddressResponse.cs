using Api.Models.Entities.Orders;

namespace Api.Contracts.Orders;

public class AddressResponse
{
  public Guid Id { get; init; }
  public Guid UserId { get; init; }
  public string? Country { get; init; }
  public string? City { get; init; }
  public string? Street { get; init; }
  public int PostalCode { get; init; }

  public static AddressResponse From(Address a) => new()
  {
    Id = a.Id,
    UserId = a.UserId,
    Country = a.Country,
    City = a.City,
    Street = a.Street,
    PostalCode = a.PostalCode,
  };
}
