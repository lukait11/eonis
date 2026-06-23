using Stripe;

namespace Api.Services;

public class StripeService
{
  public async Task<PaymentIntent> CreatePaymentIntentAsync(long amountInCents)
  {
    var options = new PaymentIntentCreateOptions
    {
      Amount = amountInCents,
      Currency = "eur",
      AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },
    };
    var service = new PaymentIntentService();
    return await service.CreateAsync(options);
  }
}
