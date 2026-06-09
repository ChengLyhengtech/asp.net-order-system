using System.ComponentModel.DataAnnotations;

namespace aps.net_order_system.DTOs
{
    public class AdminResetPasswordDto
    {
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
