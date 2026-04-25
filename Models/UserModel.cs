namespace aps.net_order_system.Models
{
    public class UserModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Identity Fields
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // Security
        public string PasswordHash { get; set; } = string.Empty;

        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}