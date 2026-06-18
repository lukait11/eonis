using Api.Data.Interfaces.Orders;
using Api.Data.Interfaces.Payments;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Payments;

[ApiController]
[Route("api/[controller]")]
public class PaymentController(
  IPaymentRepository paymentRepository,
  IOrderRepository orderRepository
) : ControllerBase
{
  
}
