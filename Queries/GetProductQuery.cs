using aps.net_order_system.Data;
using aps.net_order_system.Models;
using aps.net_order_system.DTOs; // Make sure to include this
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Queries
{
    public class GetProductQuery
    {
        public int Limit { get; set; } = 5;
        public int? CategoryId { get; set; }
        public string? SearchTerm { get; set; }
    }
    public class GetProductHandler
    {
        private readonly AppDbContext _context;
        public GetProductHandler(AppDbContext context) => _context = context;

        // Change the return type to IEnumerable<ProductDto>
        public async Task<IEnumerable<ProductDto>> Handle(GetProductQuery query)
        {
            // Start with the base queryable representation of your Products
            var queryable = _context.Products.AsQueryable();

            // 1. Filter by Category if CategoryId is provided
            if (query.CategoryId.HasValue)
            {
                queryable = queryable.Where(p => p.CategoryId == query.CategoryId.Value);
            }

            // 2. Filter by Search Term (Checks if Name or Description contains the term)
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                string term = query.SearchTerm.Trim().ToLower();
                queryable = queryable.Where(p => p.Name.ToLower().Contains(term) ||
                                                 p.Description.ToLower().Contains(term));
            }

            // 3. Project to DTO, Apply Limit, and Execute the SQL query
            return await queryable
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
                    DiscountEndDate = p.DiscountEndDate
                })
                .Take(query.Limit)
                .ToListAsync();
        }
    }
}