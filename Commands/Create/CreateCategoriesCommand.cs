using aps.net_order_system.Data;
using aps.net_order_system.DTOs;
using aps.net_order_system.Models;

namespace aps.net_order_system.Commands.Create
{
    public class CreateCategoriesCommand
    {
        private readonly AppDbContext _context;
        
        public CreateCategoriesCommand(AppDbContext context) => _context = context;

        public async Task<CategoriesModel> Handle(CategoryCreateDto command)
        {
            var categories = new CategoriesModel
            {
                ImageUrl = command.ImageUrl,
                CategoryName = command.CategoryName,
            };

            _context.Categories.Add(categories);
            await _context.SaveChangesAsync();
            return categories;
        }
    }
}
