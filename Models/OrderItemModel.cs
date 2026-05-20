using System.ComponentModel.DataAnnotations.Schema;

namespace aps.net_order_system.Models
{
    public class OrderItemModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; } // Foreign Key to OrderModel
        public int ProductId { get; set; } // Foreign Key to ProductModel
        public int Quantity { get; set; }
        public string SpecialInstructions { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }
        public string PaymentStatus { get; set; } = "Paid";

        // --- New Calculation Snapshot Fields ---
        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountApplied { get; set; } // Stores the 10% used at checkout

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtPurchase { get; set; } // Stores the final single item price ($0.90)

        // Navigation Properties
        public virtual OrderModel? Order { get; set; }
        public virtual ProductModel? Product { get; set; }
    }
}
