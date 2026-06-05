using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Queries
{
    public class GetSalesAnalyticsQuery
    {
        public int Days { get; set; } = 7;
    }

    public class GetSalesAnalyticsHandler
    {
        private readonly AppDbContext _context;

        public GetSalesAnalyticsHandler(AppDbContext context) => _context = context;

        public async Task<SalesAnalyticsDto> Handle(GetSalesAnalyticsQuery query, CancellationToken cancellationToken = default)
        {
            var startDate = DateTime.Today.AddDays(-(query.Days - 1));

            // Fetch successful orders within the timeframe
            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.PaymentStatus == "Paid")
                .ToListAsync(cancellationToken);

            // 1. Calculate KPI Summary Blocks
            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;

            // 2. Build Daily Charts
            var dailyRatesList = new List<DailyRevenueRateDto>();

            for (int i = 0; i < query.Days; i++)
            {
                var currentDate = startDate.AddDays(i);

                // Filter records belonging to this specific day
                var dayOrders = orders.Where(o => o.CreatedAt.Date == currentDate.Date).ToList();

                dailyRatesList.Add(new DailyRevenueRateDto
                {
                    DateLabel = currentDate.ToString("ddd, MMM d"),
                    TotalAmount = dayOrders.Sum(o => o.TotalAmount), // Sum of revenue for today
                    TotalOrder = dayOrders.Count                    // Count of orders for today
                });
            }

            return new SalesAnalyticsDto
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                DailyRates = dailyRatesList
            };
        }
    }
}
