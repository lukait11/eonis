using Api.Models.Entities.Catalog;

namespace Api.Models.DTO.Catalog;

public class UpdateProductRequest
{
  public Guid Id { get; set; }
  public Guid? CategoryId { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public double BasePrice { get; set; }
  public double Discount { get; set; }
  public string? Material { get; set; }
  public ProductStatus? Status { get; set; }
}
