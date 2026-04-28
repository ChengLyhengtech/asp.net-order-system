namespace aps.net_order_system.DTOs
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string SpecialInstructions { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }

        public ProductDto Product { get; set; } // ✅ ADD THIS
    }
}
