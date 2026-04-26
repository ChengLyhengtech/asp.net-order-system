using System.ComponentModel.DataAnnotations;

namespace aps.net_order_system.DTOs
{
    public class CategoryCreateDto
    {
        // We don't include ID because the DB generates it
        [Required(ErrorMessage = "Image URL is required")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category Name is required")]
        public string CategoryName { get; set; } = string.Empty;

        // Notice: No "products" list here!
    }
}
