using aps.net_order_system.Data;

namespace aps.net_order_system.Commands
{
    public class UpdateOrderStatusCommand
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateOrderStatusCommandHandler
    {
        private readonly AppDbContext _context;
        public UpdateOrderStatusCommandHandler(AppDbContext context) => _context = context;

        public async Task<bool> Handle(UpdateOrderStatusCommand command)
        {
            var order = await _context.Orders.FindAsync(command.Id);
            if (order == null) return false;

            order.Status = command.Status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}