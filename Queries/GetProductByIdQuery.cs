using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Queries
{
    public class GetProductByIdQuery
    {
        public int Id { get; set; }
    }

    public class GetProductByIdHandler
    {
        private readonly AppDbContext _context;

        public GetProductByIdHandler(AppDbContext context) => _context = context;

        // Returns a single ProductDto or null if not found
        public async Task<ProductDto?> HandleAsync(GetProductByIdQuery query)
        {
            return await _context.Products
                .Where(p => p.Id == query.Id)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    ProductImg = !string.IsNullOrEmpty(p.ProductImg)
                         ? $"/uploads/{p.ProductImg}"
                         : "/uploads/default.png",
                    Description = p.Description,
                    Price = p.Price,
                    IsAvailable = p.IsAvailable,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName,
                    DiscountPercentage = p.DiscountPercentage,
                    DiscountStartDate = p.DiscountStartDate,
                    DiscountEndDate = p.DiscountEndDate,
                    IsDiscountOverrideActive = p.IsDiscountOverrideActive,

                    // Pull calculated properties from the model
                    DiscountStatusBadge = p.DiscountStatusBadge,
                    PromoPrice = p.PromoPrice
                })
                .FirstOrDefaultAsync(); // Fetches just one item
        }
    }
}
