using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using aps.net_order_system.Models;
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Queries
{
    public class GetCategoriesQuery { }

    public class GetCategoriesHandler
    {
        private readonly AppDbContext _context;
        public GetCategoriesHandler(AppDbContext context)
        {
            _context = context;
        }

        // Change return type from Model to DTO
        public async Task<IEnumerable<CategoriesDto>> Handle(GetCategoriesQuery query)
        {
            return await _context.Categories
                .Select(c => new CategoriesDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    ImageUrl = c.ImageUrl,
                    // Map products to DTO to break the loop
                    Products = c.Products.Select(p => new ProductDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        ProductImg = p.ProductImg,
                        Description = p.Description,
                        Price = p.Price,
                        IsAvailable = p.IsAvailable,
                        CategoryId = p.CategoryId
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}