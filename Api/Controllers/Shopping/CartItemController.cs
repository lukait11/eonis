using Api.Data.Interfaces.Catalog;
using Api.Data.Interfaces.Shopping;
using Api.Models.Entities.Shopping;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Shopping;

[ApiController]
[Route("api/[controller]")]
public class CartItemController(
  ICartItemRepository cartItemRepository,
  ICartRepository cartRepository,
  IProductVariantRepository productVariantRepository
) : ControllerBase
{
  [HttpGet("{cartItemId:guid}")]
  public async Task<IActionResult> GetById(Guid cartItemId)
  {
    var cartItem = await cartItemRepository.GetCartItemByIdAsync(cartItemId);
    if (cartItem == null)
    {
      return NotFound();
    }
    return Ok(cartItem);
  }

  [HttpGet("cart/{cartId:guid}")]
  public async Task<IActionResult> GetByCartId(Guid cartId)
  {
    var cartItems = await cartItemRepository.GetCartItemsByCartIdAsync(cartId);
    if (cartItems == null || !cartItems.Any())
    {
      return NotFound();
    }
    return Ok(cartItems);
  }

  [HttpPost]
  public async Task<IActionResult> Create(CartItem cartItem)
  {
    if (await cartRepository.GetCartByIdAsync(cartItem.CartId) == null)
    {
      return NotFound("Cart not found.");
    }
    if (await productVariantRepository.GetProductVariantByIdAsync(cartItem.ProductVariantId) == null)
    {
      return NotFound("Product variant not found.");
    }
    var createdCartItem = await cartItemRepository.CreateCartItemAsync(cartItem);
    return CreatedAtAction(nameof(GetById), new { cartItemId = createdCartItem.Id }, createdCartItem);
  }

  [HttpPut]
  public async Task<IActionResult> Update(CartItem cartItem)
  {
    if (await cartRepository.GetCartByIdAsync(cartItem.CartId) == null)
    {
      return NotFound("Cart not found.");
    }
    if (await productVariantRepository.GetProductVariantByIdAsync(cartItem.ProductVariantId) == null)
    {
      return NotFound("Product variant not found.");
    }
    var updatedCartItem = await cartItemRepository.UpdateCartItemAsync(cartItem);
    if (updatedCartItem == null)
    {
      return NotFound();
    }
    return Ok(updatedCartItem);
  }

  [HttpDelete("{cartItemId:guid}")]
  public async Task<IActionResult> Delete(Guid cartItemId)
  {
    var deleted = await cartItemRepository.DeleteCartItemAsync(cartItemId);
    if (!deleted)
    {
      return NotFound();
    }
    return Ok();
  }
}
