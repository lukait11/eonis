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
    var orders = await orderRepository.GetOrdersAsync();
    if (orders == null || !orders.Any())
    {
      return NotFound();
    }
    return Ok(orders);
  }

  [HttpGet("{orderId:guid}")]
  public async Task<IActionResult> GetById(Guid orderId)
  {
    var order = await orderRepository.GetOrderByIdAsync(orderId);
    if (order == null)
    {
      return NotFound();
    }
    return Ok(order);
  }

  [HttpGet("/user/{userId:guid}")]
  public async Task<IActionResult> GetByUserId(Guid userId)
  {
    var orders = await orderRepository.GetOrdersByUserIdAsync(userId);
    if (orders == null || !orders.Any())
    {
      return NotFound();
    }
    return Ok(orders);
  }

  [HttpGet("/seller/{sellerId:guid}")]
  public async Task<IActionResult> GetBySellerId(Guid sellerId)
  {
    var orders = await orderRepository.GetOrdersBySellerIdAsync(sellerId);
    if (orders == null || !orders.Any())
    {
      return NotFound();
    }
    return Ok(orders);
  }

  [HttpGet("/status/{status}")]
  public async Task<IActionResult> GetByStatus(OrderStatus status)
  {
    var orders = await orderRepository.GetOrdersByStatusAsync(status);
    if (orders == null || !orders.Any())
    {
      return NotFound();
    }
    return Ok(orders);
  }

  [HttpPost]
  public async Task<IActionResult> Create(Order order)
  {
    if(await applicationUserRepository.GetUserByIdAsync(order.UserId) == null)
    {
      return BadRequest("User does not exist.");
    }
    if(await sellerProfileRepository.GetSellerProfileByIdAsync(order.SellerProfileId) == null)
    {
      return BadRequest("Seller profile does not exist.");
    }
    if(await addressRepository.GetAddressByIdAsync(order.AddressId) == null)
    {
      return BadRequest("Address does not exist.");
    }
    var createdOrder = await orderRepository.CreateOrderAsync(order);
    return CreatedAtAction(nameof(GetById), new { orderId = createdOrder.Id }, createdOrder);
  }

  [HttpPut]
  public async Task<IActionResult> Update(Order order)
  {
    if(await applicationUserRepository.GetUserByIdAsync(order.UserId) == null)
    {
      return BadRequest("User does not exist.");
    }
    if(await sellerProfileRepository.GetSellerProfileByIdAsync(order.SellerProfileId) == null)
    {
      return BadRequest("Seller profile does not exist.");
    }
    if(await addressRepository.GetAddressByIdAsync(order.AddressId) == null)
    {
      return BadRequest("Address does not exist.");
    }
    if (order.Id == Guid.Empty)
    {
      return BadRequest();
    }
    var updatedOrder = await orderRepository.UpdateOrderAsync(order);
    if (updatedOrder == null)
    {
      return NotFound();
    }
    return Ok(updatedOrder);
  }

  [HttpDelete("{orderId:guid}")]
  public async Task<IActionResult> Delete(Guid orderId)
  {
    var deleted = await orderRepository.DeleteOrderAsync(orderId);
    if (!deleted)
    {
      return NotFound();
    }
    return Ok();
  }
}
