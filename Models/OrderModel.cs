using System.ComponentModel.DataAnnotations.Schema;

namespace aps.net_order_system.Models
{
    public class OrderModel
    {
        public int Id { get; set; } // Primary Key (Database ID)
        public string OrderId { get; set; } = string.Empty; // e.g., "ORD-005"
        public int TableId { get; set; }
        public string Status { get; set; } = "Pending";
        public string PaymentStatus { get; set; } = "Unpaid";
        public string PaymentMethod { get; set; } = string.Empty; // Cash, KHQR
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Relationship: One Order has many Items
        public virtual ICollection<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>();
    }
}
