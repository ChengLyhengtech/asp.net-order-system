using System.ComponentModel.DataAnnotations;

namespace aps.net_order_system.Models
{
    public class PaymentModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Invoice { get; set; } = string.Empty;
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Md5 { get; set; } = string.Empty;
        // Statuses: PENDING, PAID, EXPIRED
        public string Status { get; set; } = "PENDING";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}