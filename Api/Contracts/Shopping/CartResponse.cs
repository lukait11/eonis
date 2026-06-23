using Api.Models.Entities.Shopping;

namespace Api.Contracts.Shopping;

public class CartResponse
{
  public Guid Id { get; init; }
  public Guid UserId { get; init; }
  public double TotalAmount { get; init; }
  public DateTime CreatedAt { get; init; }
  public DateTime UpdatedAt { get; init; }
  public IEnumerable<CartItemResponse> Items { get; init; } = [];

  public static CartResponse From(Cart c) => new()
  {
    Id = c.Id,
    UserId = c.UserId,
    TotalAmount = c.TotalAmount,
    CreatedAt = c.CreatedAt,
    UpdatedAt = c.UpdatedAt,
    Items = c.Items.Select(CartItemResponse.From),
  };
}
