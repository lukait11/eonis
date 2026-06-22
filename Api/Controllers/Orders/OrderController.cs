using Api.Data.Interfaces.Identity;
using Api.Data.Interfaces.Orders;
using Api.Models.Entities.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Orders;

[ApiController]
[Route("api/[controller]")]
public class OrderController(
  IOrderRepository orderRepository,
  IApplicationUserRepository applicationUserRepository,
  ISellerProfileRepository sellerProfileRepository,
  IAddressRepository addressRepository
) : ControllerBase
{
  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    try
    {
      var orders = await orderRepository.GetOrdersAsync();
      if (orders == null || !orders.Any())
        return NoContent();
      return Ok(orders);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("{orderId:guid}")]
  public async Task<IActionResult> GetById(Guid orderId)
  {
    try
    {
      var order = await orderRepository.GetOrderByIdAsync(orderId);
      if (order == null)
        return NoContent();
      return Ok(order);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("user/{userId:guid}")]
  public async Task<IActionResult> GetByUserId(Guid userId)
  {
    try
    {
      var orders = await orderRepository.GetOrdersByUserIdAsync(userId);
      if (orders == null || !orders.Any())
        return NoContent();
      return Ok(orders);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("seller/{sellerId:guid}")]
  public async Task<IActionResult> GetBySellerId(Guid sellerId)
  {
    try
    {
      var orders = await orderRepository.GetOrdersBySellerIdAsync(sellerId);
      if (orders == null || !orders.Any())
        return NoContent();
      return Ok(orders);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpGet("status/{status}")]
  public async Task<IActionResult> GetByStatus(OrderStatus status)
  {
    try
    {
      var orders = await orderRepository.GetOrdersByStatusAsync(status);
      if (orders == null || !orders.Any())
        return NoContent();
      return Ok(orders);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(Order order)
  {
    try
    {
      if (await applicationUserRepository.GetUserByIdAsync(order.UserId) == null)
        return BadRequest("User does not exist.");
      if (await sellerProfileRepository.GetSellerProfileByIdAsync(order.SellerProfileId) == null)
        return BadRequest("Seller profile does not exist.");
      if (await addressRepository.GetAddressByIdAsync(order.AddressId) == null)
        return BadRequest("Address does not exist.");
      var createdOrder = await orderRepository.CreateOrderAsync(order);
      return CreatedAtAction(nameof(GetById), new { orderId = createdOrder.Id }, createdOrder);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut]
  public async Task<IActionResult> Update(Order order)
  {
    try
    {
      if (order.Id == Guid.Empty)
        return BadRequest();
      if (await applicationUserRepository.GetUserByIdAsync(order.UserId) == null)
        return BadRequest("User does not exist.");
      if (await sellerProfileRepository.GetSellerProfileByIdAsync(order.SellerProfileId) == null)
        return BadRequest("Seller profile does not exist.");
      if (await addressRepository.GetAddressByIdAsync(order.AddressId) == null)
        return BadRequest("Address does not exist.");
      var updatedOrder = await orderRepository.UpdateOrderAsync(order);
      if (updatedOrder == null)
        return NotFound();
      return Ok(updatedOrder);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpDelete("{orderId:guid}")]
  public async Task<IActionResult> Delete(Guid orderId)
  {
    try
    {
      var deleted = await orderRepository.DeleteOrderAsync(orderId);
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
