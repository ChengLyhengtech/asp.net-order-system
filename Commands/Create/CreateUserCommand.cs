using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using aps.net_order_system.Models;

namespace aps.net_order_system.Commands.Create
{
    public class CreateUserHandler
    {
        private readonly AppDbContext _context;
        public CreateUserHandler(AppDbContext context) => _context = context;

        public async Task<UserModel> Handle(UserDto command)
        {
            var newUser = new UserModel
            {
                // Mapping the data correctly
                Username = command.Username,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber, // Map Phone
                PasswordHash = command.Password, // Map Password (ideally hashed)

                // Automatic data
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }
    }
}