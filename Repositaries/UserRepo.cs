//using aps.net_order_system.Data;
//using aps.net_order_system.Interface;
//using aps.net_order_system.Models;
//using Microsoft.EntityFrameworkCore;

//namespace aps.net_order_system.Repositaries
//{
//    public class UserRepo : IUserService // Implementing the interface
//    {
//        private readonly AppDbContext _context;
//        public UserRepo(AppDbContext context)
//        {
//            _context = context;
//        }
//        public async Task<IEnumerable<UserModel>> GetUsersAsync()
//        {
//            return await _context.Users.ToListAsync();
//        }
//        public async Task<UserModel?> GetUserByIdAsync(string id)
//        {
//            return await _context.Users.FindAsync(id);
//        }
//        public async Task<UserModel> AddUserAsync(UserModel user)
//        {
//            await _context.Users.AddAsync(user);
//            await _context.SaveChangesAsync();
//            return user;
//        }
//        public async Task<bool> UpdateUserAsync(string id, UserModel user)
//        {
//            var existingUser = await _context.Users.FindAsync(id);
//            if (existingUser == null) return false;

//            // Map updated values
//            existingUser.Username = user.Username; // Assuming these fields exist
//            // existingUser.Email = user.Email; 

//            await _context.SaveChangesAsync();
//            return true;
//        }
//        public async Task<bool> DeleteUserAsync(string id)
//        {
//            var user = await _context.Users.FindAsync(id);
//            if (user == null) return false;

//            _context.Users.Remove(user);
//            await _context.SaveChangesAsync();
//            return true;
//        }
//    }
//}