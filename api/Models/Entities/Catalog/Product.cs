namespace Api.Models.Entities.Catalog;

public class Product
{
  public Guid Id { get; set; }
  public Guid SellerId { get; set; }
  public Guid CategoryId { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public double BasePrice { get; set; }
  public double Discount { get; set; }
  public string? Material { get; set; }
  public string? Status { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}
