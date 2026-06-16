using Api.Context;
using Api.Data.Interfaces.Payments;
using Api.Models.Entities.Payments;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Implementations.Payments;

public class PaymentRepository(DatabaseContext context) : IPaymentRepository
{
  public async Task<Payment> CreatePaymentAsync(Payment payment)
  {
    payment.CreatedAt = DateTime.UtcNow;
    context.Payments.Add(payment);
    await context.SaveChangesAsync();
    return payment;
  }

  public async Task<bool> DeletePaymentAsync(Guid paymentId)
  {
    var payment = await GetPaymentByIdAsync(paymentId);
    if (payment == null) return false;

    context.Payments.Remove(payment);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<Payment?> GetPaymentByIdAsync(Guid paymentId)
  {
    return await context.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);
  }

  public async Task<Payment> GetPaymentByOrderIdAsync(Guid orderId)
  {
    var payment = await context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId) 
      ?? throw new InvalidOperationException("Payment not found");

    return payment;
  }

  public async Task<IEnumerable<Payment>> GetPaymentsAsync()
  {
    return await context.Payments.ToListAsync();
  }

  public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
  {
    return await context.Payments.Where(p => p.Status == status).ToListAsync();
  }

  public async Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(Guid userId)
  {
    var OrderIds = await context.Orders.Where(o => o.UserId == userId).Select(o => o.Id).ToListAsync();
    return await context.Payments.Where(p => OrderIds.Contains(p.OrderId)).ToListAsync();
  }

  public async Task<Payment?> UpdatePaymentAsync(Payment payment)
  {
    context.Payments.Update(payment);
    await context.SaveChangesAsync();
    return payment;
  }
}