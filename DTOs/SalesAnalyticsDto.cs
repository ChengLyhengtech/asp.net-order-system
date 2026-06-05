namespace aps.net_order_system.DTOs
{
    public class SalesAnalyticsDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public List<DailyRevenueRateDto> DailyRates { get; set; } = new();
    }

    public class DailyRevenueRateDto
    {
        public string DateLabel { get; set; } = string.Empty; // e.g., "Sun, Mar 8"
        public decimal TotalAmount { get; set; }              // Revenue for this day
        public int TotalOrder { get; set; }                    // Count of orders for this day
    }
}
