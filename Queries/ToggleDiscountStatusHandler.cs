using aps.net_order_system.Data;
using MediatR;

namespace aps.net_order_system.Queries
{
    public class ToggleDiscountStatusCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public bool IsDiscountOverrideActive { get; set; }
    }
    public class ToggleDiscountStatusHandler : IRequestHandler<ToggleDiscountStatusCommand, bool>
    {
        private readonly AppDbContext _context;

        public ToggleDiscountStatusHandler(AppDbContext context) => _context = context;

        public async Task<bool> Handle(ToggleDiscountStatusCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
            if (product == null) return false;

            // Flip the toggle switch state
            product.IsDiscountOverrideActive = request.IsDiscountOverrideActive;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
