namespace aps.net_order_system.DTOs
{
    public class CreatePaymentRequestDto
    {
        public string? OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime? ExpiryDate { get; set; }
    }

    public class PaymentResponseDto
    {
        public string QrString { get; set; } = string.Empty;
        public string Md5 { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
    }

    public class CheckTransactionResponseDto
    {
        public BakongStatus? Status { get; set; }
        public BakongCheckDataDto? Data { get; set; }
    }

    public class BakongStatus
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class BakongCheckDataDto
    {
        public string Status { get; set; } = string.Empty; // SUCCESS / FAILED
        public string Hash { get; set; } = string.Empty;
    }

    public class DeeplinkRequest
    {
        public string QrString { get; set; } = string.Empty;
    }
}