using System.ComponentModel.DataAnnotations;

public class PaymentModel
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Invoice { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Md5 { get; set; } = string.Empty;
    public string QrString { get; set; } = string.Empty;
    public string Status { get; set; } = "PENDING";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}