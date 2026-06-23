using Api.Models.Entities.Catalog;

namespace Api.Contracts.Catalog;

public class CategoryResponse
{
  public Guid Id { get; init; }
  public string? Name { get; init; }
  public string? Description { get; init; }

  public static CategoryResponse From(Category c) => new()
  {
    Id = c.Id,
    Name = c.Name,
    Description = c.Description,
  };
}
