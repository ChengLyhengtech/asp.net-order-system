using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using aps.net_order_system.Models;

namespace aps.net_order_system.Commands.Create
{
    public class CreateProductCommand
    {
        private readonly AppDbContext _context;
        public CreateProductCommand(AppDbContext context) => _context = context; 
        public async Task<ProductModel> Handle(ProductDto command)
        {
            var product = new ProductModel
            {
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                IsAvailable = command.IsAvailable,
                CategoryId = command.CategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }
    }
}
