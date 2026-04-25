using System.ComponentModel.DataAnnotations;

namespace aps.net_order_system.Models
{
    public class CategoriesModel
    {
        [Key] // Tells EF Core this is your Primary Key
        public int Id { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;
        public ICollection<ProductModel> Products { get; set; } = new List<ProductModel>();
    }
}
