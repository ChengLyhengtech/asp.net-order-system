namespace aps.net_order_system.DTOs
{
    public class UpdateDiscountStatusDto
    {
        // True = Active (If within date), False = Suspended manually
        public bool IsDiscountOverrideActive { get; set; }
    }
}
