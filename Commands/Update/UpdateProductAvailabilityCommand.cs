using aps.net_order_system.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace aps.net_order_system.Commands.Update
{
    public class UpdateProductAvailabilityCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class UpdateProductAvailabilityHandler : IRequestHandler<UpdateProductAvailabilityCommand, bool>
    {
        private readonly AppDbContext _context;
        public UpdateProductAvailabilityHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateProductAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
            if (product == null)
            {
                return false; // Product not found
            }

            product.IsAvailable = request.IsAvailable;
            await _context.SaveChangesAsync(cancellationToken);

            return true; // Update successful
        }
    }
}
