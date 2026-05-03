using aps.net_order_system.DTOs;

namespace aps.net_order_system.Interface
{
    public interface IPaymentService
    {
        // Generates the KHQR string and MD5 hash
        PaymentResponseDto GenerateKhqr(CreatePaymentRequestDto request);

        // Polls the Bakong API to check if a payment was successful
        Task<CheckTransactionResponseDto> CheckPaymentStatusAsync(string md5);
        Task<object> GenerateDeeplinkAsync(string qrString);
    }
}