using aps.net_order_system.DTOs;

namespace aps.net_order_system.Interface
{
    public interface IPaymentService
    {
        PaymentResponseDto GenerateKhqr(CreatePaymentRequestDto request);
        // Changed to allow null if request fails
        Task<CheckTransactionResponseDto?> CheckPaymentStatusAsync(string md5);
        Task<object> GenerateDeeplinkAsync(string qrString);
    }
}