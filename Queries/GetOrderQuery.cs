using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Queries
{
    // The Query (The Request)
    public class GetOrderQuery
    {
        public int Id { get; set; }
        public GetOrderQuery(int id) => Id = id;
    }

    // The Handler (The Logic)
    public class GetOrderQueryHandler
    {
        private readonly AppDbContext _context;
        public GetOrderQueryHandler(AppDbContext context) => _context = context;

        public async Task<OrderDto?> Handle(GetOrderQuery query)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderId = o.OrderId,
                    TableId = o.TableId,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt,

                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        Subtotal = oi.Subtotal,
                        SpecialInstructions = oi.SpecialInstructions, // 👈 Added this
                        Product = new ProductDto
                        {
                            Id = oi.Product.Id,
                            Name = oi.Product.Name,
                            ProductImg = oi.Product.ProductImg,    // 👈 Map the image
                            Description = oi.Product.Description, // 👈 Map description
                            Price = oi.Product.Price,              // 👈 Map price
                            IsAvailable = oi.Product.IsAvailable, // 👈 Map availability
                            CategoryId = oi.Product.CategoryId     // 👈 Map category
                        }
                    }).ToList()
                })
                .FirstOrDefaultAsync(o => o.Id == query.Id);
        }
    }
}