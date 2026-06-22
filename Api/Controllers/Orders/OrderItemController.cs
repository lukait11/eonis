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
    var orderItem = await orderItemRepository.GetOrderItemByIdAsync(orderItemId);
    if (orderItem == null)
    {
      return NoContent();
    }
    return Ok(orderItem);
  }

  [HttpGet("order/{orderId:guid}")]
  public async Task<IActionResult> GetByOrderId(Guid orderId)
  {
    var orderItems = await orderItemRepository.GetOrderItemsByOrderIdAsync(orderId);
    if (orderItems == null || !orderItems.Any())
    {
      return NoContent();
    }
    return Ok(orderItems);
  }

  [HttpPost]
  public async Task<IActionResult> Create(OrderItem orderItem)
  {
    if(await orderRepository.GetOrderByIdAsync(orderItem.OrderId) == null)
    {
      return BadRequest("Order does not exist.");
    }
    if(await productVariantRepository.GetProductVariantByIdAsync(orderItem.ProductVariantId) == null)
    {
      return BadRequest("Product variant does not exist.");
    }
    var createdOrderItem = await orderItemRepository.CreateOrderItemAsync(orderItem);
    return CreatedAtAction(nameof(GetById), new { orderItemId = createdOrderItem.Id }, createdOrderItem);
  }

  [HttpPut]
  public async Task<IActionResult> Update(OrderItem orderItem)
  {
    if (orderItem.Id == Guid.Empty)
    {
      return BadRequest();
    }
    if(await orderRepository.GetOrderByIdAsync(orderItem.OrderId) == null)
    {
      return BadRequest("Order does not exist.");
    }
    if(await productVariantRepository.GetProductVariantByIdAsync(orderItem.ProductVariantId) == null)
    {
      return BadRequest("Product variant does not exist.");
    }
    var updatedOrderItem = await orderItemRepository.UpdateOrderItemAsync(orderItem);
    if (updatedOrderItem == null)
    {
      return NotFound();
    }
    return Ok(updatedOrderItem);
  }

  [HttpDelete("{orderItemId:guid}")]
  public async Task<IActionResult> Delete(Guid orderItemId)
  {
    var deleted = await orderItemRepository.DeleteOrderItemAsync(orderItemId);
    if (!deleted)
    {
      return NotFound();
    }
    return Ok();
  }
}
