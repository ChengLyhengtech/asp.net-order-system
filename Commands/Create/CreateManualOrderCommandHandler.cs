using aps.net_order_system.Data;
using aps.net_order_system.Models;
using MediatR;

namespace aps.net_order_system.Commands.Create
{
    public class CreateManualOrderCommandHandler : IRequestHandler<CreateManualOrderCommand, int>
    {
        private readonly AppDbContext _context;

        public CreateManualOrderCommandHandler(AppDbContext context)
        {
            _context = context;
        }
    
        public async Task<int> Handle(CreateManualOrderCommand request, CancellationToken cancellationToken)
        {
            string orderStatus = string.Equals(request.PaymentStatus, "PAID", StringComparison.OrdinalIgnoreCase)
                   ? "PAID" : "UNPAID";

            var newOrder = new OrderModel
            {
                TableId = request.TableId,
                Status = orderStatus,
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync(cancellationToken);

            return newOrder.Id;
        }


    }
}
