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
        // Add this to fix the error
        public int ResponseCode { get; set; }

        public string? ResponseMessage { get; set; }

        public BakongData? Data { get; set; }
    }
    public class BakongData
    {
        public string? Hash { get; set; }
        public string? Status { get; set; }
        // Add other fields from Bakong as needed
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