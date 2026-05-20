using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aps.net_order_system.Models
{
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
    }
}