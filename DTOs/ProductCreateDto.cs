using System.ComponentModel.DataAnnotations;

namespace aps.net_order_system.DTOs
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Img Url is required")]
        public string ProductImg { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product Name is required")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(0.01, float.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public float Price { get; set; }

        public bool IsAvailable { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
