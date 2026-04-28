using aps.net_order_system.Data;

namespace aps.net_order_system.Commands
{
    public class DeleteOrderCommand
    {
        public int Id { get; set; }
    }

    public class DeleteOrderCommandHandler
    {
        private readonly AppDbContext _context;
        public DeleteOrderCommandHandler(AppDbContext context) => _context = context;

        public async Task<bool> Handle(DeleteOrderCommand command)
        {
            var order = await _context.Orders.FindAsync(command.Id);
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}