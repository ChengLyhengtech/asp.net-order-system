using System.ComponentModel.DataAnnotations;

namespace aps.net_order_system.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Img Url is required")]
        public string ProductImg { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Price { get; set; }
        public bool IsAvailable { get; set; }
        public int CategoryId { get; set; }
        // Add this to send the calculated chart value to the UI

        public string CategoryName { get; set; }
        public decimal DisplayValue { get; set; }

        public decimal DiscountPercentage { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }
        public bool IsDiscountOverrideActive { get; set; }
        // Calculated fields to match the UI requirements
        public string DiscountStatusBadge { get; set; } = string.Empty;
        public float PromoPrice { get; set; }
    }
}