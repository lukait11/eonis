using Api.Models.Entities.Payments;

namespace Api.Data.Interfaces.Payments;

public interface IPaymentRepository
{
  Task<IEnumerable<Payment>> GetPaymentsAsync();
  Task<Payment?> GetPaymentByIdAsync(Guid paymentId);
  Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(Guid userId);
  Task<Payment> GetPaymentByOrderIdAsync(Guid orderId);
  Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status);
  Task<Payment> CreatePaymentAsync(Payment payment);
  Task<Payment?> UpdatePaymentAsync(Payment payment);
  Task<bool> DeletePaymentAsync(Guid paymentId);
}
