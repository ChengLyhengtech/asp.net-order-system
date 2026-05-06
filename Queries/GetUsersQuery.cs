using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using aps.net_order_system.Models;
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Queries
{
    public class GetUsersQuery
    {
        // This is a simple request object. 
        // If you had a search filter, you'd put properties here.
    }

    public class GetUsersHandler
    {
        private readonly AppDbContext _context;
        public GetUsersHandler(AppDbContext context) => _context = context;

        // Change return type to IEnumerable<UserDto>
        public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery query)
        {
            return await _context.Users
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    FullName = user.FullName ?? "",
                    CreatedAt = user.CreatedAt,
                    // This "magic" join gets the role names for each user
                    Roles = _context.UserRoles
                        .Where(ur => ur.UserId == user.Id)
                        .Join(_context.Roles,
                              ur => ur.RoleId,
                              r => r.Id,
                              (ur, r) => r.Name!)
                        .ToList()
                })
                .ToListAsync();
        }
    }
}