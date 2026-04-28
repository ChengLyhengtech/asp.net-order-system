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

        // Navigation Properties
        public virtual OrderModel? Order { get; set; }
        public virtual ProductModel? Product { get; set; }
    }
}
