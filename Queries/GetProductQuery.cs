using aps.net_order_system.Data;
using aps.net_order_system.Models;
using aps.net_order_system.DTOs; // Make sure to include this
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Queries
{
    public class GetProductQuery
    {
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
                    ProductImg = p.ProductImg,
                    Description = p.Description,
                    Price = p.Price,
                    IsAvailable = p.IsAvailable,
                    CategoryId = p.CategoryId
                })
                .ToListAsync();
        }
    }
}