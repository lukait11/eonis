namespace Api.Contracts.Payment;

public class PaymentIntentResponse
{
  public string ClientSecret { get; init; } = string.Empty;
  public string PaymentIntentId { get; init; } = string.Empty;
}
