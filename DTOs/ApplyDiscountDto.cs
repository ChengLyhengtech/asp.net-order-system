namespace aps.net_order_system.DTOs
{
    public class ApplyDiscountDto
    {
        public decimal DiscountPercentage { get; set; }
        public DateTime DiscountStartDate { get; set; }
        public DateTime DiscountEndDate { get; set; }
    }
}
