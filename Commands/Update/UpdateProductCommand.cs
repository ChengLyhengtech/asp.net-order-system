using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Commands.Update
{
    public class UpdateProductCommand
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ProductImg { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Price { get; set; }
        public bool IsAvailable { get; set; }
        public int CategoryId { get; set; }
    }

    public class UpdateProductHandler
    {
        private readonly AppDbContext _context;

        public UpdateProductHandler(AppDbContext context) => _context = context;

        public async Task<ProductDto?> HandleAsync(UpdateProductCommand command)
        {
            var product = await _context.Products.FindAsync(command.Id);

            if (product == null)
                return null;

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == command.CategoryId);
            if (!categoryExists)
                throw new Exception("The specified CategoryId does not exist.");

            product.Name = command.Name;
            product.ProductImg = command.ProductImg;
            product.Description = command.Description;
            product.Price = command.Price;
            product.IsAvailable = command.IsAvailable;
            product.CategoryId = command.CategoryId;

            await _context.SaveChangesAsync();

            // ✅ Map to DTO
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                ProductImg = product.ProductImg,
                Description = product.Description,
                Price = product.Price,
                IsAvailable = product.IsAvailable,
                CategoryId = product.CategoryId
            };
        }
    }
}