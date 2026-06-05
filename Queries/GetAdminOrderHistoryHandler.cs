using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Queries
{
    public class GetAdminOrderHistoryHandler
    {
        private readonly AppDbContext _context;

        public GetAdminOrderHistoryHandler(AppDbContext context) => _context = context;

        public async Task<IEnumerable<OrderDto>> Handle(GetAdminOrderHistoryQuery query)
        {
            var queryable = _context.Orders
                .Include(o => o.OrderItems) // Include items if your OrderDto displays them
                .AsQueryable();

            // 1. Apply Date Filtering
            switch (query.DateFilter)
            {
                case OrderDateFilter.Today:
                    var today = DateTime.Today;
                    queryable = queryable.Where(o => o.CreatedAt >= today);
                    break;

                case OrderDateFilter.Last7Days:
                    var sevenDaysAgo = DateTime.Today.AddDays(-7);
                    queryable = queryable.Where(o => o.CreatedAt >= sevenDaysAgo);
                    break;

                case OrderDateFilter.AllTime:
                default:
                    // Do nothing, return everything
                    break;
            }

            // 2. Apply Search Filtering (OrderId or TableId)
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var cleanSearch = query.SearchTerm.Trim().ToLower();

                // Check if the user is typing an integer (likely searching for Table ID)
                bool isNumeric = int.TryParse(cleanSearch, out int searchTableId);

                if (isNumeric)
                {
                    queryable = queryable.Where(o => o.OrderId.ToLower().Contains(cleanSearch) ||
                                                     o.TableId == searchTableId.ToString());
                }
                else
                {
                    queryable = queryable.Where(o => o.OrderId.ToLower().Contains(cleanSearch));
                }
            }

            // 3. Project to OrderDto and return (Ordered by newest first)
            return await queryable
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderId = o.OrderId,
                    TableId = o.TableId,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    PaymentMethod = o.PaymentMethod,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt
                })
                .Take(query.Limit)
                .ToListAsync();
        }
    }
}
