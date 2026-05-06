namespace aps.net_order_system.DTOs
{
    public class OrderListResponseDto
    {
        public int TotalCount { get; set; }
        public IEnumerable<OrderDto> Orders { get; set; }
    }
}
