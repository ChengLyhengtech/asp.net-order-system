using System.ComponentModel.DataAnnotations;

namespace aps.net_order_system.DTOs
{
    public class ProductUpdateDto
    {
        // Optional: Only provided if the user wants to upload a NEW image
        public IFormFile? ProductImg { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(0.01, float.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public float Price { get; set; }

        public bool IsAvailable { get; set; }

        [Required]
        public int CategoryId { get; set; }

        // Add discount fields here if you want your update endpoint to handle them!
    }
}
