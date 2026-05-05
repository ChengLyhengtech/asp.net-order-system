using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Queries
{
    public class GetStaffHistoryHandler
    {
        private readonly AppDbContext _context;

        public GetStaffHistoryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDto>> Handle(DateTime? from = null, DateTime? to = null)
        {
            var start = from ?? DateTime.UtcNow.Date.AddDays(-1);
            var end = to ?? DateTime.UtcNow.Date.AddDays(1);

            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.CreatedAt >= start && o.CreatedAt < end)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderId = o.OrderId,
                    TableId = o.TableId,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt,
                    Items = o.OrderItems.Select(i => new OrderItemDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        SpecialInstructions = i.SpecialInstructions,
                        Subtotal = i.Subtotal
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}