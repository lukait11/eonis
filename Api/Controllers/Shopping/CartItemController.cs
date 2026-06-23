using Api.Contracts.Shopping;
using Api.Data.Interfaces.Catalog;
using Api.Data.Interfaces.Shopping;
using Api.Models.DTO.Shopping;
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
    try
    {
      var item = await cartItemRepository.GetCartItemByIdAsync(cartItemId);
      if (item == null) return NotFound();
      return Ok(CartItemResponse.From(item));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("cart/{cartId:guid}")]
  public async Task<IActionResult> GetByCartId(Guid cartId)
  {
    try
    {
      var items = await cartItemRepository.GetCartItemsByCartIdAsync(cartId);
      return Ok(items.Select(CartItemResponse.From));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(CreateCartItemRequest request)
  {
    try
    {
      if (await cartRepository.GetCartByIdAsync(request.CartId) == null)
        return NotFound("Cart not found.");
      if (await productVariantRepository.GetProductVariantByIdAsync(request.ProductVariantId) == null)
        return NotFound("Product variant not found.");

      var item = new CartItem
      {
        CartId = request.CartId,
        ProductVariantId = request.ProductVariantId,
        Quantity = request.Quantity,
      };

      var created = await cartItemRepository.CreateCartItemAsync(item);
      return CreatedAtAction(nameof(GetById), new { cartItemId = created.Id }, CartItemResponse.From(created));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut("{cartItemId:guid}")]
  public async Task<IActionResult> Update(Guid cartItemId, UpdateCartItemRequest request)
  {
    try
    {
      if (await cartRepository.GetCartByIdAsync(request.CartId) == null)
        return NotFound("Cart not found.");
      if (await productVariantRepository.GetProductVariantByIdAsync(request.ProductVariantId) == null)
        return NotFound("Product variant not found.");

      var updated = await cartItemRepository.UpdateCartItemAsync(new CartItem
      {
        Id = cartItemId,
        CartId = request.CartId,
        ProductVariantId = request.ProductVariantId,
        Quantity = request.Quantity,
      });
      if (updated == null) return NotFound();
      return Ok(CartItemResponse.From(updated));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("{cartItemId:guid}")]
  public async Task<IActionResult> Delete(Guid cartItemId)
  {
    try
    {
      var deleted = await cartItemRepository.DeleteCartItemAsync(cartItemId);
      if (!deleted) return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("cart/{cartId:guid}")]
  public async Task<IActionResult> ClearCart(Guid cartId)
  {
    try
    {
      await cartItemRepository.DeleteCartItemsByCartIdAsync(cartId);
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
