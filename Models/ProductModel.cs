using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aps.net_order_system.Models
{
    // 1. Define the Discount Status Enum
    public enum DiscountStatus
    {
        Active,
        Expired,
        Suspended
    }
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Img Url is required")]
        public string ProductImg { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Price { get; set; }
        public bool IsAvailable { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public CategoriesModel? Category { get; set; }

        // discount
        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercentage { get; set; }

        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }


        /// <summary>
        /// Maps to the "Override Status" toggle switch in image_c59e26.png.
        /// If false, the store owner has manually suspended/turned off the discount.
        /// </summary>
        public bool IsDiscountOverrideActive { get; set; } = true;

        /// <summary>
        /// Calculates the dynamic string status based on dates and the manual override switch.
        /// </summary>
        [NotMapped]
        public string DiscountStatusBadge
        {
            get
            {
                if (DiscountPercentage <= 0 || !DiscountStartDate.HasValue || !DiscountEndDate.HasValue)
                {
                    return "Expired"; // No discount configured
                }

                // If the owner explicitly flipped the toggle switch off
                if (!IsDiscountOverrideActive)
                {
                    return DiscountStatus.Suspended.ToString();
                }

                var now = DateTime.UtcNow;

                if (now < DiscountStartDate.Value)
                {
                    return "Upcoming"; // Optional: if the discount is set for a future date
                }
                if (now > DiscountEndDate.Value)
                {
                    return DiscountStatus.Expired.ToString();
                }

                return DiscountStatus.Active.ToString();
            }
        }

        /// <summary>
        /// Calculates the "Promo Price" column automatically.
        /// </summary>
        [NotMapped]
        public float PromoPrice
        {
            get
            {
                // Only apply discount if the calculated status badge says it is currently Active
                if (DiscountStatusBadge == "Active")
                {
                    // Convert Price to decimal to avoid precision issues during math, then back to float
                    decimal originalPrice = (decimal)Price;
                    decimal discountAmount = originalPrice * (DiscountPercentage / 100);
                    return (float)(originalPrice - discountAmount);
                }

                return Price; // Return regular price if discount is Expired or Suspended
            }
        }

        // Dynamically checks if the discount is active today
        public bool IsDiscountActive
        {
            get
            {
                var now = DateTime.UtcNow; // Use UtcNow to avoid timezone headaches
                return DiscountPercentage > 0
                       && DiscountStartDate <= now
                       && DiscountEndDate >= now;
            }
        }
    }
}