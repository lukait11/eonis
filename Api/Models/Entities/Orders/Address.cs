using System.Text.Json.Serialization;
using Api.Models.Entities.Identity;

namespace Api.Models.Entities.Orders;

public class Address
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public string? Country { get; set; }
  public string? City { get; set; }
  public string? Street { get; set; }
  public int PostalCode { get; set; }

  [JsonIgnore]
  // Navigation properties
  public ApplicationUser? User { get; set; }
}