using aps.net_order_system.Data;
using aps.net_order_system.Models;
using aps.net_order_system.DTOs; // Make sure to include this
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Queries
{
    public class GetProductQuery
    {
        public int Limit { get; set; } = 5;
    }
    public class GetProductHandler
    {
        private readonly AppDbContext _context;
        public GetProductHandler(AppDbContext context) => _context = context;

        // Change the return type to IEnumerable<ProductDto>
        public async Task<IEnumerable<ProductDto>> Handle(GetProductQuery query)
        {
            return await _context.Products
                .Select(p => new ProductDto
                {
                    // Map the properties manually
                    Id = p.Id,
                    Name = p.Name,
                    ProductImg = !string.IsNullOrEmpty(p.ProductImg)
                         ? $"/uploads/{p.ProductImg}"
                         : "/uploads/default.png",
                    Description = p.Description,
                    Price = p.Price,
                    IsAvailable = p.IsAvailable,
                    CategoryId = p.CategoryId,

                    DiscountPercentage = p.DiscountPercentage,
                    DiscountStartDate = p.DiscountStartDate,
                    DiscountEndDate = p.DiscountEndDate
                })
                .ToListAsync();
        }
    }
}