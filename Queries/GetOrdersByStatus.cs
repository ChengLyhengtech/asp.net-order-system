using MediatR;
using Microsoft.EntityFrameworkCore;
using aps.net_order_system.Data;
using aps.net_order_system.DTOs;

namespace aps.net_order_system.Queries
{
    // The Query (The Request)
    public class GetOrdersByStatusQuery : IRequest<List<OrderDto>>
    {
        public string Status { get; set; }

        public GetOrdersByStatusQuery(string status)
        {
            Status = status;
        }
    }

    // The Handler (The Logic)
    public class GetOrdersByStatusHandler : IRequestHandler<GetOrdersByStatusQuery, List<OrderDto>>
    {
        private readonly AppDbContext _context;

        public GetOrdersByStatusHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDto>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .Where(o => o.Status == request.Status)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderId = o.OrderId,
                    Status = o.Status,
                    TableId = o.TableId,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}