using Api.Contracts.Wishlists;
using Api.Data.Interfaces.Identity;
using Api.Data.Interfaces.Wishlists;
using Api.Models.DTO.Wishlists;
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
    try
    {
      var wishlist = await wishlistRepository.GetWishlistByIdAsync(wishlistId);
      if (wishlist == null) return NotFound();
      return Ok(WishlistResponse.From(wishlist));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("user/{userId:guid}")]
  public async Task<IActionResult> GetUserWishlist(Guid userId)
  {
    try
    {
      var wishlist = await wishlistRepository.GetWishlistByUserIdAsync(userId);
      if (wishlist == null) return NotFound();
      return Ok(WishlistResponse.From(wishlist));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(CreateWishlistRequest request)
  {
    try
    {
      if (await userRepository.GetUserByIdAsync(request.UserId) == null)
        return BadRequest("User not found.");

      var wishlist = new Wishlist { UserId = request.UserId };
      var created = await wishlistRepository.CreateWishlistAsync(wishlist);
      return CreatedAtAction(nameof(GetWishlist), new { wishlistId = created.Id }, WishlistResponse.From(created));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("{wishlistId:guid}")]
  public async Task<IActionResult> Delete(Guid wishlistId)
  {
    try
    {
      var deleted = await wishlistRepository.DeleteWishlistAsync(wishlistId);
      if (!deleted) return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
