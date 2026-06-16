using Api.Models.Entities.Orders;

namespace Api.Models.Entities.Payments;

public class Payment
{
  public Guid Id { get; set; }
  public Guid OrderId { get; set; }
  public Guid StripePaymentIntentId { get; set; }
  public double Amount { get; set; }
  public string Currency { get; set; } = "RSD";
  public PaymentStatus Status { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  // Navigation properties
  public Order? Order { get; set; }
}