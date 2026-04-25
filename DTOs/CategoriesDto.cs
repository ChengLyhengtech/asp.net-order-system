using System.ComponentModel.DataAnnotations;

namespace aps.net_order_system.DTOs
{
    public class CategoriesDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Image URL is required")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        // Add this line to store the products in your DTO
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}