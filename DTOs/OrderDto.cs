namespace aps.net_order_system.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public int TableId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } // ✅ ADD THIS
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
