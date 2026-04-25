using aps.net_order_system.Data;
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

        public async Task<IEnumerable<UserModel>> Handle(GetUsersQuery query)
        {
            return await _context.Users.ToListAsync();
        }
    }
}