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

        public async Task<CategoriesDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            // Fetch the single matching category entity by its ID
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (category == null) return null;

            // Map Entity -> DTO directly (No products loaded)
            return new CategoriesDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                ImageUrl = !string.IsNullOrEmpty(category.ImageUrl)
                     ? $"/uploads/{category.ImageUrl}"
                     : "/uploads/default-category.png"
            };
        }
    }
}