using aps.net_order_system.Data;

namespace aps.net_order_system.Commands.Update
{
    // 1. This is the "Job Description" (The data needed for the update)
    public class UpdateCategoriesCommand
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }

    // 2. This is the "Worker" (The logic that performs the update)
    public class UpdateCategoriesHandler
    {
        private readonly AppDbContext _context;

        public UpdateCategoriesHandler(AppDbContext context) => _context = context;

        public async Task<bool> HandleAsync(UpdateCategoriesCommand command)
        {
            // Find the existing category by the ID provided in the command
            var category = await _context.Categories.FindAsync(command.Id);

            if (category == null)
            {
                return false; // Category doesn't exist
            }

            category.ImageUrl = command.ImageUrl;
            category.CategoryName = command.CategoryName;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}