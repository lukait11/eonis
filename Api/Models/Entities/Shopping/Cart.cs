using Api.Models.Entities.Identity;

namespace Api.Models.Entities.Shopping;

public class Cart
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public double TotalAmount { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; }

  // Navigation properties
  public ICollection<CartItem> Items { get; set; } = [];
  public ApplicationUser? User { get; set; }
}