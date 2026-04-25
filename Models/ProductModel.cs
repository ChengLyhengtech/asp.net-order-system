using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aps.net_order_system.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Price { get; set; }
        public bool IsAvailable { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public CategoriesModel? Category { get; set; }
    }
}