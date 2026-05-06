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
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();

            if (from.HasValue)
            {
                var start = DateTime.SpecifyKind(from.Value, DateTimeKind.Utc);
                query = query.Where(o => o.CreatedAt >= start);
            }

            if (to.HasValue)
            {
                var end = DateTime.SpecifyKind(to.Value, DateTimeKind.Utc);
                query = query.Where(o => o.CreatedAt <= end);
            }

            return await query
                .OrderByDescending(o => o.CreatedAt)
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
                        Subtotal = i.Subtotal,
                        Product = i.Product == null ? null : new ProductDto
                        {
                            Id = i.Product.Id,
                            Name = i.Product.Name,
                            Price = i.Product.Price
                        }
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}