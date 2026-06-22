using Api.Data.Interfaces.Identity;
using Api.Data.Interfaces.Wishlists;
using Api.Models.Entities.Wishlists;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Wishlists;

[ApiController]
[Route("api/[controller]")]
public class WishlistController(
  IWishlistRepository wishlistRepository,
  IApplicationUserRepository userRepository
) : ControllerBase
{
  [HttpGet("{wishlistId:guid}")]
  public async Task<IActionResult> GetWishlist(Guid wishlistId)
  {
    var wishlist = await wishlistRepository.GetWishlistByIdAsync(wishlistId);
    if (wishlist == null)
    {
      return NotFound();
    }
    return Ok(wishlist);
  }

  [HttpGet("user/{userId:guid}")]
  public async Task<IActionResult> GetUserWishlist(Guid userId)
  {
    var wishlist = await wishlistRepository.GetWishlistByUserIdAsync(userId);
    if (wishlist == null)
    {
      return NotFound();
    }
    return Ok(wishlist);
  }

  [HttpPost]
  public async Task<IActionResult> CreateWishlist(Wishlist wishlist)
  {
    if (await userRepository.GetUserByIdAsync(wishlist.UserId) == null)
    {
      return BadRequest("User not found.");
    }
    var createdWishlist = await wishlistRepository.CreateWishlistAsync(wishlist);
    return CreatedAtAction(nameof(GetWishlist), new { wishlistId = createdWishlist.Id }, createdWishlist);
  }

  [HttpPut]
  public async Task<IActionResult> UpdateWishlist(Wishlist wishlist)
  {
    if (await userRepository.GetUserByIdAsync(wishlist.UserId) == null)
    {
      return BadRequest("User not found.");
    }
    var updatedWishlist = await wishlistRepository.UpdateWishlistAsync(wishlist);
    if (updatedWishlist == null)
    {
      return NotFound();
    }
    return Ok(updatedWishlist);
  }

  [HttpDelete("{wishlistId:guid}")]
  public async Task<IActionResult> DeleteWishlist(Guid wishlistId)
  {
    var deleted = await wishlistRepository.DeleteWishlistAsync(wishlistId);
    if (!deleted)
    {
      return NotFound();
    }
    return Ok();
  }
}
