namespace Api.Models.DTO.Shopping;

public class UpdateCartItemRequest
{
  public Guid Id { get; set; }
  public Guid CartId { get; set; }
  public Guid ProductVariantId { get; set; }
  public int Quantity { get; set; }
}
