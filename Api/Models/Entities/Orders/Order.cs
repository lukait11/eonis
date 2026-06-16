using Api.Models.Entities.Identity;

namespace Api.Models.Entities.Orders;

public class Order
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public Guid AddressId { get; set; }
  public Guid SellerProfileId { get; set; }
  public OrderStatus Status { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public double BaseAmount { get; set; }
  public double Discount { get; set; }

  //  Navigation properties
  public ApplicationUser? User { get; set; }
  public Address? Address { get; set; }
  public ICollection<OrderItem> Items { get; set; } = [];
}