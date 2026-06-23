using Api.Contracts.Payment;
using Api.Data.Interfaces.Orders;
using Api.Data.Interfaces.Payments;
using Api.Models.DTO.Payment;
using Api.Models.Entities.Orders;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using PaymentEntity = Api.Models.Entities.Payments.Payment;
using PaymentStatusEntity = Api.Models.Entities.Payments.PaymentStatus;

namespace Api.Controllers.Payment;

[ApiController]
[Route("api/[controller]")]
public class PaymentController(
  IOrderRepository orderRepository,
  IPaymentRepository paymentRepository,
  StripeService stripeService,
  IConfiguration configuration
) : ControllerBase
{
  [Authorize]
  [HttpPost("create-intent")]
  public async Task<IActionResult> CreateIntent([FromBody] CreatePaymentIntentRequest request)
  {
    try
    {
      var order = await orderRepository.GetOrderByIdAsync(request.OrderId);
      if (order == null) return NotFound("Order not found.");

      // Treat RSD amounts as EUR cents for test mode (1 RSD ≈ 1 cent)
      var total = order.BaseAmount - order.Discount;
      var amountInCents = Math.Max(50L, (long)Math.Round(total));

      var intent = await stripeService.CreatePaymentIntentAsync(amountInCents);

      var payment = new PaymentEntity
      {
        OrderId = order.Id,
        StripePaymentIntentId = intent.Id,
        Amount = total,
        Currency = "eur",
        Status = PaymentStatusEntity.Pending,
      };
      await paymentRepository.CreatePaymentAsync(payment);

      return Ok(new PaymentIntentResponse
      {
        ClientSecret = intent.ClientSecret,
        PaymentIntentId = intent.Id,
      });
    }
    catch (StripeException ex)
    {
      return BadRequest(ex.StripeError?.Message ?? ex.Message);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPost("webhook")]
  public async Task<IActionResult> Webhook()
  {
    try
    {
      var json = await new StreamReader(Request.Body).ReadToEndAsync();
      var signature = Request.Headers["Stripe-Signature"].ToString();
      var webhookSecret = configuration["Stripe:WebhookSecret"]!;

      Event stripeEvent;
      try
      {
        stripeEvent = EventUtility.ConstructEvent(json, signature, webhookSecret);
      }
      catch (StripeException)
      {
        return BadRequest("Invalid webhook signature.");
      }

      if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
      {
        var intent = stripeEvent.Data.Object as PaymentIntent;
        if (intent is not null)
        {
          var payment = await paymentRepository.GetPaymentByStripeIdAsync(intent.Id);
          if (payment is not null)
          {
            payment.Status = PaymentStatusEntity.Succeeded;
            await paymentRepository.UpdatePaymentAsync(payment);

            if (payment.Order is not null)
            {
              payment.Order.Status = OrderStatus.Paid;
              await orderRepository.UpdateOrderAsync(payment.Order);
            }
          }
        }
      }
      else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
      {
        var intent = stripeEvent.Data.Object as PaymentIntent;
        if (intent is not null)
        {
          var payment = await paymentRepository.GetPaymentByStripeIdAsync(intent.Id);
          if (payment is not null)
          {
            payment.Status = PaymentStatusEntity.Failed;
            await paymentRepository.UpdatePaymentAsync(payment);
          }
        }
      }

      return Ok();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}
