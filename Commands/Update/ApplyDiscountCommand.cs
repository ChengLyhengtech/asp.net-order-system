using MediatR;

namespace aps.net_order_system.Commands.Update
{
    public class ApplyDiscountCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime DiscountStartDate { get; set; }
        public DateTime DiscountEndDate { get; set; }
    }
}
