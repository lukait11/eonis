using Api.Contracts.Wishlists;
using Api.Data.Interfaces.Catalog;
using Api.Data.Interfaces.Wishlists;
using Api.Models.DTO.Wishlists;
using Api.Models.Entities.Wishlists;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Wishlists;

[ApiController]
[Route("api/[controller]")]
public class WishlistItemController(
  IWishlistItemRepository wishlistItemRepository,
  IWishlistRepository wishlistRepository,
  IProductRepository productRepository
) : ControllerBase
{
  [HttpGet("{wishlistItemId:guid}")]
  public async Task<IActionResult> GetWishlistItem(Guid wishlistItemId)
  {
    try
    {
      var item = await wishlistItemRepository.GetWishlistItemByIdAsync(wishlistItemId);
      if (item == null) return NotFound();
      return Ok(WishlistItemResponse.From(item));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("wishlist/{wishlistId:guid}")]
  public async Task<IActionResult> GetWishlistItems(Guid wishlistId)
  {
    try
    {
      var items = await wishlistItemRepository.GetWishlistItemsByWishlistIdAsync(wishlistId);
      return Ok(items.Select(WishlistItemResponse.From));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> CreateWishlistItem(CreateWishlistItemRequest request)
  {
    try
    {
      if (await wishlistRepository.GetWishlistByIdAsync(request.WishlistId) == null)
        return NotFound("Wishlist not found.");
      if (await productRepository.GetProductByIdAsync(request.ProductId) == null)
        return NotFound("Product not found.");

      var item = new WishlistItem
      {
        WishlistId = request.WishlistId,
        ProductId = request.ProductId,
        AddedAt = DateTime.UtcNow,
      };

      var created = await wishlistItemRepository.CreateWishlistItemAsync(item);
      return CreatedAtAction(nameof(GetWishlistItem), new { wishlistItemId = created.Id }, WishlistItemResponse.From(created));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("{wishlistItemId:guid}")]
  public async Task<IActionResult> DeleteWishlistItem(Guid wishlistItemId)
  {
    try
    {
      var deleted = await wishlistItemRepository.DeleteWishlistItemAsync(wishlistItemId);
      if (!deleted) return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
