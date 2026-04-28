using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Queries
{
    public class GetTopProductQuery
    {
        // Allow the user to specify how many top products to return
        public int Limit { get; set; } = 5;
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
            // Raw SQL for performance. 
            // This joins Products with OrderItems to find the most sold items.
            var sql = @"
                SELECT TOP ({0}) p.*
                FROM Products p
                INNER JOIN (
                    SELECT ProductId, SUM(Quantity) as TotalSold
                    FROM OrderItems
                    GROUP BY ProductId
                ) oi ON p.Id = oi.ProductId
                ORDER BY oi.TotalSold DESC";
            // Executing via EF Core but using raw SQL for speed
            var topProducts = await _context.Products
                .FromSqlRaw(sql, query.Limit)
                .AsNoTracking() // Improves speed by disabling change tracking
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                    // Map other fields as necessary
                })
                .ToListAsync();
            return topProducts;
        }
    }
}