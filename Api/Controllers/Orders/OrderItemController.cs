using Api.Data.Interfaces.Catalog;
using Api.Data.Interfaces.Orders;
using Api.Models.Entities.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Orders;

[ApiController]
[Route("api/[controller]")]
public class OrderItemController(
  IOrderItemRepository orderItemRepository,
  IOrderRepository orderRepository,
  IProductVariantRepository productVariantRepository
) : ControllerBase
{
  [HttpGet("{orderItemId:guid}")]
  public async Task<IActionResult> GetById(Guid orderItemId)
  {
    try
    {
      var orderItem = await orderItemRepository.GetOrderItemByIdAsync(orderItemId);
      if (orderItem == null)
        return NoContent();
      return Ok(orderItem);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("order/{orderId:guid}")]
  public async Task<IActionResult> GetByOrderId(Guid orderId)
  {
    try
    {
      var orderItems = await orderItemRepository.GetOrderItemsByOrderIdAsync(orderId);
      if (orderItems == null || !orderItems.Any())
        return NoContent();
      return Ok(orderItems);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(OrderItem orderItem)
  {
    try
    {
      if (await orderRepository.GetOrderByIdAsync(orderItem.OrderId) == null)
        return BadRequest("Order does not exist.");
      if (await productVariantRepository.GetProductVariantByIdAsync(orderItem.ProductVariantId) == null)
        return BadRequest("Product variant does not exist.");
      var createdOrderItem = await orderItemRepository.CreateOrderItemAsync(orderItem);
      return CreatedAtAction(nameof(GetById), new { orderItemId = createdOrderItem.Id }, createdOrderItem);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut]
  public async Task<IActionResult> Update(OrderItem orderItem)
  {
    try
    {
      if (orderItem.Id == Guid.Empty)
        return BadRequest();
      if (await orderRepository.GetOrderByIdAsync(orderItem.OrderId) == null)
        return BadRequest("Order does not exist.");
      if (await productVariantRepository.GetProductVariantByIdAsync(orderItem.ProductVariantId) == null)
        return BadRequest("Product variant does not exist.");
      var updatedOrderItem = await orderItemRepository.UpdateOrderItemAsync(orderItem);
      if (updatedOrderItem == null)
        return NotFound();
      return Ok(updatedOrderItem);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("{orderItemId:guid}")]
  public async Task<IActionResult> Delete(Guid orderItemId)
  {
    try
    {
      var deleted = await orderItemRepository.DeleteOrderItemAsync(orderItemId);
      if (!deleted)
        return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
