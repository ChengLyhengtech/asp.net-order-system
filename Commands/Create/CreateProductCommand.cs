using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using aps.net_order_system.Models;

namespace aps.net_order_system.Commands.Create
{
    public class CreateProductCommand
    {
        private readonly AppDbContext _context;
        public CreateProductCommand(AppDbContext context) => _context = context;
        public async Task<ProductModel> Handle(ProductCreateDto command)
        {
            string fileName = "";

            if (command.ProductImg != null)
            {
                var folder = Path.Combine("wwwroot", "images");
                Directory.CreateDirectory(folder);

                fileName = Guid.NewGuid().ToString() +
                           Path.GetExtension(command.ProductImg.FileName);

                var path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await command.ProductImg.CopyToAsync(stream);
                }
            }

            var product = new ProductModel
            {
                Name = command.Name,
                ProductImg = fileName, // ✅ save string only
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
