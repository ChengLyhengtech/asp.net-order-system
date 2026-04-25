using aps.net_order_system.Data;

namespace aps.net_order_system.Commands.Delete
{
    public class DeleteUserCommand
    {
        public string Id { get; set; } = string.Empty;
    }

    public class DeleteUserHandler
    {
        private readonly AppDbContext _context;
        public DeleteUserHandler(AppDbContext context) => _context = context;

        public async Task<bool> HandleAsync(DeleteUserCommand command)
        {
            var user = await _context.Users.FindAsync(command.Id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}