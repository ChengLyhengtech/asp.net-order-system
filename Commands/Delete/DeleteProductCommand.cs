using aps.net_order_system.Data;
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Commands.Delete
{
    public class DeleteProductCommand
    {
        public int Id { get; set; }
    }

    public class DeleteProductHandler
    {
        private readonly AppDbContext _context;

        // Constructor to inject the Database Context
        public DeleteProductHandler(AppDbContext context) => _context = context;

        public async Task<bool> HandleAsync(DeleteProductCommand command)
        {
            // 1. Find the product by its ID
            var product = await _context.Products.FindAsync(command.Id);

            // 2. If the product doesn't exist, return false
            if (product == null)
            {
                return false;
            }
            // 3. Remove it from the database
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}