namespace Api.Models.DTO.Catalog;

public class UpdateCategoryRequest
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public Guid? ParentCategoryId { get; set; }
}
