using aps.net_order_system.Data;

namespace aps.net_order_system.Commands.Update
{
    public class UpdateUserCommand
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        // Add other fields you want to allow updating
    }

    public class UpdateUserHandler
    {
        private readonly AppDbContext _context;
        public UpdateUserHandler(AppDbContext context) => _context = context;

        public async Task<bool> HandleAsync(UpdateUserCommand command)
        {
            var user = await _context.Users.FindAsync(command.Id);
            if (user == null) return false;

            user.Username = command.Username;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}