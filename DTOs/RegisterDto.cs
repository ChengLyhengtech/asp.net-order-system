using System.ComponentModel.DataAnnotations;

namespace aps.net_order_system.DTOs
{
    // Public registration - no Role field exposed or required
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // Admin-only registration for creating Staff or other Admins
    public class RegisterManagementDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required] // Role is mandatory here
        public string Role { get; set; } = string.Empty;
    }
}
