using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore; // Required for FirstOrDefaultAsync

namespace aps.net_order_system.Queries
{
    // 1. THE QUERY (The "What I want")
    public class TotalCountOrderQuery : IRequest<TotalCountOrderDto>
    {
        // This is just a simple data contract
    }

    // 2. THE HANDLER (The "How I get it")
    // Note: You MUST add ": IRequestHandler<TotalCountOrderQuery, TotalCountOrderDto>" here
    public class TotalCountOrderHandler : IRequestHandler<TotalCountOrderQuery, TotalCountOrderDto>
    {
        private readonly AppDbContext _context;

        public TotalCountOrderHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TotalCountOrderDto> Handle(TotalCountOrderQuery request, CancellationToken cancellationToken)
        {
            // Get the record from your database
            var data = await _context.TotalCountOrders.FirstOrDefaultAsync(cancellationToken);

            return new TotalCountOrderDto
            {
                TotalCount = data?.TotalCount ?? 0
            };
        }
    }
}