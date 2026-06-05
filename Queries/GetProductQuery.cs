using aps.net_order_system.Data;
using aps.net_order_system.Models;
using aps.net_order_system.DTOs; // Make sure to include this
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Queries
{
    public class GetProductQuery
    {
        public int Limit { get; set; } = 5;
        public int? CategoryId { get; set; }
        public string? SearchTerm { get; set; }
        public string? DiscountStatus { get; set; }
    }
    public class GetProductHandler
    {
        private readonly AppDbContext _context;
        public GetProductHandler(AppDbContext context) => _context = context;

        public async Task<IEnumerable<ProductDto>> Handle(GetProductQuery query)
        {
            var queryable = _context.Products.AsQueryable();

            // 1. Filter by Category if CategoryId is provided
            if (query.CategoryId.HasValue)
            {
                queryable = queryable.Where(p => p.CategoryId == query.CategoryId.Value);
            }

            // 2. Filter by Search Term
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                string term = query.SearchTerm.Trim().ToLower();
                queryable = queryable.Where(p => p.Name.ToLower().Contains(term) ||
                                                 p.Description.ToLower().Contains(term));
            }

            // 3. Filter by Discount Status (Database-level logic mirroring your model properties)
            if (!string.IsNullOrWhiteSpace(query.DiscountStatus))
            {
                var status = query.DiscountStatus.Trim().ToLower();
                var now = DateTime.UtcNow;

                if (status == "active")
                {
                    // Active means: Has discount percentage, owner hasn't toggled it off, and current time falls within window
                    queryable = queryable.Where(p => p.DiscountPercentage > 0
                                                 && p.IsDiscountOverrideActive
                                                 && p.DiscountStartDate <= now
                                                 && p.DiscountEndDate >= now);
                }
                else if (status == "suspended")
                {
                    // Suspended means: Has configured discount dates, but the owner flipped the toggle switch off
                    queryable = queryable.Where(p => p.DiscountPercentage > 0
                                                 && p.DiscountStartDate != null
                                                 && p.DiscountEndDate != null
                                                 && !p.IsDiscountOverrideActive);
                }
                else if (status == "upcoming")
                {
                    // Upcoming means: Has a discount configured, override is active, but the start time is still in the future
                    queryable = queryable.Where(p => p.DiscountPercentage > 0
                                                 && p.IsDiscountOverrideActive
                                                 && p.DiscountStartDate > now);
                }
                else if (status == "expired")
                {
                    // Expired means: No discount configuration OR the end date has completely passed
                    queryable = queryable.Where(p => p.DiscountPercentage <= 0
                                                 || p.DiscountStartDate == null
                                                 || p.DiscountEndDate == null
                                                 || p.DiscountEndDate < now);
                }
            }

            // 4. Project to DTO, Apply Limit, and Execute the SQL query
            return await queryable
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    ProductImg = !string.IsNullOrEmpty(p.ProductImg)
                         ? $"/uploads/{p.ProductImg}"
                         : "/uploads/default.png",
                    Description = p.Description,
                    Price = p.Price,
                    IsAvailable = p.IsAvailable,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName,

                    // Discount Mapping
                    DiscountPercentage = p.DiscountPercentage,
                    DiscountStartDate = p.DiscountStartDate,
                    DiscountEndDate = p.DiscountEndDate,
                    IsDiscountOverrideActive = p.IsDiscountOverrideActive,

                    // Pull calculated properties from the model instance
                    DiscountStatusBadge = p.DiscountStatusBadge,
                    PromoPrice = p.PromoPrice
                })
                .Take(query.Limit)
                .ToListAsync();
        }
    }
}