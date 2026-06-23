namespace Api.Models.DTO.Catalog;

public class UpdateProductVariantRequest
{
  public Guid Id { get; set; }
  public Guid ProductId { get; set; }
  public string? Size { get; set; }
  public string? Color { get; set; }
  public int Quantity { get; set; }
}
