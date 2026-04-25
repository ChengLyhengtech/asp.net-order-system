//using aps.net_order_system.Interface;
//using aps.net_order_system.Models;

//namespace aps.net_order_system.Services
//{
//    public class UserService : IUserService
//    {
//        private readonly IUserService _userRepo;

//        // Inject the Repository via the Interface
//        public UserService(IUserService userRepo)
//        {
//            _userRepo = userRepo;
//        }

//        public async Task<IEnumerable<UserModel>> GetUsersAsync()
//        {
//            // Business logic could go here (e.g., filtering inactive users)
//            return await _userRepo.GetUsersAsync();
//        }

//        public async Task<UserModel?> GetUserByIdAsync(string id)
//        {
//            return await _userRepo.GetUserByIdAsync(id);
//        }

//        public async Task<UserModel> AddUserAsync(UserModel user)
//        {
//            // Logic example: Hash the password before saving
//            // user.Password = HashPassword(user.Password);

//            return await _userRepo.AddUserAsync(user);
//        }

//        public async Task<bool> UpdateUserAsync(string id, UserModel user)
//        {
//            return await _userRepo.UpdateUserAsync(id, user);
//        }

//        public async Task<bool> DeleteUserAsync(string id)
//        {
//            return await _userRepo.DeleteUserAsync(id);
//        }
//    }
//}