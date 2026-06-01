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
                // 1. Point to the 'uploads' folder you just created/configured
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                fileName = Guid.NewGuid().ToString() + Path.GetExtension(command.ProductImg.FileName);
                var path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await command.ProductImg.CopyToAsync(stream);
                }
            }

            var product = new ProductModel
            {
                Name = command.Name,
                ProductImg = fileName, // We save just the filename "guid.jpg"
                Description = command.Description,
                Price = command.Price,
                IsAvailable = command.IsAvailable,
                CategoryId = command.CategoryId,

                DiscountPercentage = command.DiscountPercentage,
                DiscountStartDate = command.DiscountStartDate,
                DiscountEndDate = command.DiscountEndDate
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }
    }
}
