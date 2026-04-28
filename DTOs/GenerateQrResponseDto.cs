namespace aps.net_order_system.DTOs
{
    public class GenerateQrResponseDto
    {
        public string TableId { get; set; } = string.Empty;
        public string EncryptedUrl { get; set; } = string.Empty;
        public string QrCodeImageBase64 { get; set; } = string.Empty;
    }
}