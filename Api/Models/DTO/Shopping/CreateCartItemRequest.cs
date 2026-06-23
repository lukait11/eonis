namespace Api.Models.DTO.Shopping;

public class CreateCartItemRequest
{
  public Guid CartId { get; set; }
  public Guid ProductVariantId { get; set; }
  public int Quantity { get; set; }
}
