namespace aps.net_order_system.DTOs
{
    // For incoming requests from your frontend
    //Folder name Payment-Geteway.cs
    public class CreatePaymentRequestDto
    {
        public string? OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD"; // Default to USD
        public DateTime ExpiryDate { get; set; }
    }

    // For returning the generated QR to your frontend
    public class PaymentResponseDto
    {
        public string QrString { get; set; } = string.Empty;
        public string Md5 { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
    }

    // For handling the nested data from Bakong status check
    public class BakongTransactionData
    {
        public string Hash { get; set; } = string.Empty;
        public string FromAccountId { get; set; } = string.Empty;
        public string ToAccountId { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public double CreatedDateMs { get; set; }
    }

    public class CheckTransactionResponseDto
    {
        public int ResponseCode { get; set; } // 0 = API Success
        public string ResponseMessage { get; set; } = string.Empty;
        public BakongTransactionData? Data { get; set; }
    }
}
