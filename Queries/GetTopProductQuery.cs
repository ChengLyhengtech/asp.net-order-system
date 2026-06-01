using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Queries
{
    public class GetTopProductQuery
    {
        // Allow the user to specify how many top products to return
        public int Limit { get; set; } = 5;
        public string SortBy { get; set; } = "qty"; // "qty" or "revenue"
    }

    public class GetTopProductHandler
    {
        private readonly AppDbContext _context;

        public GetTopProductHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetTopProductQuery query)
        {
            // 1. Start with the Products table
            var baseQuery = _context.Products.AsNoTracking();

            // 2. Project into our DTO while calculating aggregates from OrderItems
            var aggregatedQuery = baseQuery.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                ProductImg = p.ProductImg,
                Description = p.Description,
                Price = p.Price,
                IsAvailable = p.IsAvailable,
                CategoryId = p.CategoryId,

                // Dynamically calculate value based on the requested metric
                DisplayValue = query.SortBy.ToLower() == "revenue"
                    ? (decimal)_context.OrderItems.Where(oi => oi.ProductId == p.Id).Sum(oi => oi.Quantity * oi.Subtotal)
                    : _context.OrderItems.Where(oi => oi.ProductId == p.Id).Sum(oi => oi.Quantity)
            });

            // 3. Apply the sorting based on the requested metric and apply the limit
            return await aggregatedQuery
                .OrderByDescending(p => p.DisplayValue)
                .Take(query.Limit)
                .ToListAsync();
        }
    }
}