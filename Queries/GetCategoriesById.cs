using MediatR;
using Microsoft.EntityFrameworkCore;
using aps.net_order_system.Models;
using aps.net_order_system.DTOs;
using aps.net_order_system.Data;

namespace aps.net_order_system.Queries
{
    public record GetCategoryByIdQuery(int Id) : IRequest<CategoriesDto>;

    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoriesDto>
    {
        private readonly AppDbContext _context;

        public GetCategoryByIdHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CategoriesDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Fetch the Entity AND include the related Products
            var category = await _context.Categories
                .Include(c => c.Products) // This ensures products are loaded from the DB
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (category == null) return null;

            // 2. Map Entity -> DTO (including the nested list)
            return new CategoriesDto
            {
                Id = category.Id,
                // Note: Ensure your Entity property names match (e.g., category.CategoryName vs category.Name)
                CategoryName = category.CategoryName,
                ImageUrl = category.ImageUrl,
                Products = category.Products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                    // Map other Product fields here...
                }).ToList()
            };
        }
    }
}