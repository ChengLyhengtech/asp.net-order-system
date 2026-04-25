using aps.net_order_system.Data;
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Commands.Update
{
    public class UpdateProductCommand
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Price { get; set; }
        public bool IsAvailable { get; set; }
        public int CategoryId { get; set; }
    }

    public class UpdateProductHandler
    {
        private readonly AppDbContext _context;

        public UpdateProductHandler(AppDbContext context) => _context = context;

        public async Task<bool> HandleAsync(UpdateProductCommand command)
        {
            // 1. Find the existing product
            var product = await _context.Products.FindAsync(command.Id);

            if (product == null)
            {
                return false;
            }

            // 2. Optional: Check if the new CategoryId exists before updating
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == command.CategoryId);
            if (!categoryExists)
            {
                throw new Exception("The specified CategoryId does not exist.");
            }

            // 3. Update the fields
            product.Name = command.Name;
            product.Description = command.Description;
            product.Price = command.Price;
            product.IsAvailable = command.IsAvailable;
            product.CategoryId = command.CategoryId;

            // 4. Save changes
            await _context.SaveChangesAsync();
            return true;
        }
    }
}