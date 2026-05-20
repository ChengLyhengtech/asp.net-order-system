using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
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
                    ImageUrl = !string.IsNullOrEmpty(c.ImageUrl)
                         ? $"/uploads/{c.ImageUrl}"
                         : "/uploads/default-category.png"
                })
                .ToListAsync();
        }
    }
}