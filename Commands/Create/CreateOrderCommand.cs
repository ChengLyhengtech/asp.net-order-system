using aps.net_order_system.Data;
using aps.net_order_system.Models;

namespace aps.net_order_system.Commands
{
    public class CreateOrderCommand
    {
        public int TableId { get; set; }
        // You can add more initial fields here if needed
    }

    public class CreateOrderCommandHandler
    {
        private readonly AppDbContext _context;
        public CreateOrderCommandHandler(AppDbContext context) => _context = context;

        public async Task<int> Handle(CreateOrderCommand command)
        {
            var order = new OrderModel
            {
                OrderId = $"ORD-{Guid.NewGuid().ToString()[..5].ToUpper()}",
                TableId = command.TableId,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order.Id; // Return the new ID so the frontend can redirect
        }
    }
}