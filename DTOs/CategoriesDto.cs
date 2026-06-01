using System.ComponentModel.DataAnnotations;

namespace aps.net_order_system.DTOs
{
    public class CategoriesDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;
    }
}