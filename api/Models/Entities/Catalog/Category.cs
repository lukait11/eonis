namespace Api.Models.Entities.Catalog;

public class Category
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public Guid? ParentCategoryId { get; set; }
}
