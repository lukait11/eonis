namespace Api.Models.DTO.Orders;

public class CreateAddressRequest
{
  public Guid UserId { get; set; }
  public string? Country { get; set; }
  public string? City { get; set; }
  public string? Street { get; set; }
  public int PostalCode { get; set; }
}
