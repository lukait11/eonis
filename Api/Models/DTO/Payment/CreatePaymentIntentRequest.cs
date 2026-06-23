namespace Api.Models.DTO.Payment;

public class CreatePaymentIntentRequest
{
  public Guid OrderId { get; set; }
}
