using aps.net_order_system.Commands.Update;
using aps.net_order_system.Data;
using MediatR;

namespace aps.net_order_system.Queries
{
    public class ApplyDiscountHandler : IRequestHandler<ApplyDiscountCommand, bool>
    {
        private readonly AppDbContext _context;
        public ApplyDiscountHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> Handle(ApplyDiscountCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);

            if (product == null) return false;

            // Update only discount properties
            product.DiscountPercentage = request.DiscountPercentage;
            product.DiscountStartDate = request.DiscountStartDate;
            product.DiscountEndDate = request.DiscountEndDate;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
