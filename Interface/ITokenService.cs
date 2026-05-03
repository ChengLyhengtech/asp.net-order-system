using aps.net_order_system.Models;

namespace aps.net_order_system.Interface
{
    public interface ITokenService
    {
        Task<string> CreateToken(UserModel user);
    }
}
