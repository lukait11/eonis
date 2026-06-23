using Api.Contracts.Orders;
using Api.Data.Interfaces.Catalog;
using Api.Data.Interfaces.Identity;
using Api.Data.Interfaces.Orders;
using Api.Models.DTO.Orders;
using Api.Models.Entities.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Orders;

[ApiController]
[Route("api/[controller]")]
public class OrderController(
  IOrderRepository orderRepository,
  IApplicationUserRepository applicationUserRepository,
  ISellerProfileRepository sellerProfileRepository,
  IAddressRepository addressRepository,
  IOrderItemRepository orderItemRepository,
  IProductVariantRepository productVariantRepository
) : ControllerBase
{
  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    try
    {
      var orders = await orderRepository.GetOrdersAsync();
      return Ok(orders.Select(OrderResponse.From));
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
      if (order == null) return NotFound();
      return Ok(OrderResponse.From(order));
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
      return Ok(orders.Select(OrderResponse.From));
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
      return Ok(orders.Select(OrderResponse.From));
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
      return Ok(orders.Select(OrderResponse.From));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost]
  public async Task<IActionResult> Create(CreateOrderRequest request)
  {
    try
    {
      if (await applicationUserRepository.GetUserByIdAsync(request.UserId) == null)
        return BadRequest("User does not exist.");
      if (await sellerProfileRepository.GetSellerProfileByIdAsync(request.SellerProfileId) == null)
        return BadRequest("Seller profile does not exist.");
      if (await addressRepository.GetAddressByIdAsync(request.AddressId) == null)
        return BadRequest("Address does not exist.");

      var order = new Order
      {
        UserId = request.UserId,
        SellerProfileId = request.SellerProfileId,
        AddressId = request.AddressId,
        BaseAmount = request.BaseAmount,
        Discount = request.Discount,
        Status = request.Status,
      };

      var created = await orderRepository.CreateOrderAsync(order);
      return CreatedAtAction(nameof(GetById), new { orderId = created.Id }, OrderResponse.From(created));
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPut("{orderId:guid}/status")]
  public async Task<IActionResult> UpdateStatus(Guid orderId, UpdateOrderStatusRequest request)
  {
    try
    {
      var existing = await orderRepository.GetOrderByIdAsync(orderId);
      if (existing == null) return NotFound();

      var previousStatus = existing.Status;
      existing.Status = request.Status;

      if (previousStatus != OrderStatus.Paid && request.Status == OrderStatus.Paid)
      {
        var items = await orderItemRepository.GetOrderItemsByOrderIdAsync(orderId);
        foreach (var item in items)
        {
          var variant = await productVariantRepository.GetProductVariantByIdAsync(item.ProductVariantId);
          if (variant != null)
          {
            variant.Quantity = Math.Max(0, variant.Quantity - item.Quantity);
            await productVariantRepository.UpdateProductVariantAsync(variant);
          }
        }
      }

      var updated = await orderRepository.UpdateOrderAsync(existing);
      return Ok(OrderResponse.From(updated!));
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
      if (!deleted) return NotFound();
      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
