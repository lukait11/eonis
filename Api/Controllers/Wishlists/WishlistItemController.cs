using Api.Data.Interfaces.Catalog;
using Api.Data.Interfaces.Wishlists;
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
    var wishlistItem = await wishlistItemRepository.GetWishlistItemByIdAsync(wishlistItemId);
    if (wishlistItem == null)
    {
      return NotFound();
    }
    return Ok(wishlistItem);
  }

  [HttpGet("wishlist/{wishlistId:guid}")]
  public async Task<IActionResult> GetWishlistItems(Guid wishlistId)
  {
    var wishlistItems = await wishlistItemRepository.GetWishlistItemsByWishlistIdAsync(wishlistId);
    if (!wishlistItems.Any())
    {
      return NotFound();
    }
    return Ok(wishlistItems);
  }

  [HttpPost]
  public async Task<IActionResult> CreateWishlistItem(WishlistItem wishlistItem)
  {
    if (await wishlistRepository.GetWishlistByIdAsync(wishlistItem.WishlistId) == null)
    {
      return NotFound("Wishlist not found.");
    }
    if (await productRepository.GetProductByIdAsync(wishlistItem.ProductId) == null)
    {
      return NotFound("Product not found.");
    }
    var createdWishlistItem = await wishlistItemRepository.CreateWishlistItemAsync(wishlistItem);
    return CreatedAtAction(nameof(GetWishlistItem), new { wishlistItemId = createdWishlistItem.Id }, createdWishlistItem);
  }

  [HttpPut]
  public async Task<IActionResult> UpdateWishlistItem(WishlistItem wishlistItem)
  {
    if (await wishlistRepository.GetWishlistByIdAsync(wishlistItem.WishlistId) == null)
    {
      return NotFound("Wishlist not found.");
    }
    if (await productRepository.GetProductByIdAsync(wishlistItem.ProductId) == null)
    {
      return NotFound("Product not found.");
    }
    var updatedWishlistItem = await wishlistItemRepository.UpdateWishlistItemAsync(wishlistItem);
    if (updatedWishlistItem == null)
    {
      return NotFound();
    }
    return Ok(updatedWishlistItem);
  }

  [HttpDelete("{wishlistItemId:guid}")]
  public async Task<IActionResult> DeleteWishlistItem(Guid wishlistItemId)
  {
    var deleted = await wishlistItemRepository.DeleteWishlistItemAsync(wishlistItemId);
    if (!deleted)
    {
      return NotFound();
    }
    return Ok();
  }
}
