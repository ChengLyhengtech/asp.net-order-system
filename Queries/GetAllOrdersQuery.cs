using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Queries
{
    // 1. The Query: No parameters needed because we want everything
    public class GetAllOrdersQuery
    {
    }
    // 2. The Handler: Fetches the entire list
    public class GetAllOrdersQueryHandler
    {
        private readonly AppDbContext _context;

        public GetAllOrdersQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDto>> Handle(GetAllOrdersQuery query)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt) // Good practice: newest orders first
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderId = o.OrderId,
                    TableId = o.TableId,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        Subtotal = oi.Subtotal,
                        SpecialInstructions = oi.SpecialInstructions,
                        Product = new ProductDto
                        {
                            Id = oi.Product.Id,
                            Name = oi.Product.Name,
                            Price = oi.Product.Price
                        }
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}