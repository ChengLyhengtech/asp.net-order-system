using MediatR;

namespace aps.net_order_system.Commands.Create
{
    public class CreateManualOrderCommand : IRequest<int>
    {
        public int TableId { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
    }
}
