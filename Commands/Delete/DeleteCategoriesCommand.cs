using aps.net_order_system.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace aps.net_order_system.Commands.Delete
{
    public class DeleteCategoriesCommand
    {
        public int Id { get; set; }
    }
    public class DeleteCategoriesHandler
    {
        private readonly AppDbContext _context;
        public DeleteCategoriesHandler(AppDbContext context) => _context = context;

        public async Task<bool> HandleAsync(DeleteCategoriesCommand command)
        {
            var category = await _context.Categories.FindAsync(command.Id);

            if (category == null)
            {
                return false;
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
