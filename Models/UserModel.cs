using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace aps.net_order_system.Models
{
    public class UserModel:IdentityUser
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}