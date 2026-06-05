using System.ComponentModel.DataAnnotations;

namespace aps.net_order_system.Models
{
    public class TableQrCodeModel
    {
        [Key]
        public string TableId { get; set; } = string.Empty; // Acts as the Primary Key
        public string EncryptedUrl { get; set; } = string.Empty;
        public string QrCodeImageBase64 { get; set; } = string.Empty;
        // --- SOFT DELETE FLAG ---
        public bool IsDeleted { get; set; } = false;

        // Navigation property: One table can have many orders over time
        public ICollection<OrderModel> Orders { get; set; } = new List<OrderModel>();
    }
}
