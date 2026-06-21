using Api.Data.Interfaces.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
public class ProductImageController(
  IProductImageRepository productImageRepository,
  IProductRepository productRepository,
  IProductVariantRepository productVariantRepository
) : ControllerBase
{
  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetImage(Guid id)
  {
    var image = await productImageRepository.GetProductImageByIdAsync(id);
    if (image == null)
    {
      return NotFound();
    }

    return Ok(image);
  }
}
