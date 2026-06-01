using aps.net_order_system.Data;
using Microsoft.AspNetCore.Hosting; // Required for IWebHostEnvironment
using aps.net_order_system.DTOs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace aps.net_order_system.Commands.Update
{
    public class UpdateProductCommand
    {
        [JsonIgnore] // Prevents Swagger/JSON from expecting Id in the body
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(0.01, float.MaxValue)]
        public float Price { get; set; }

        public bool IsAvailable { get; set; }

        [Required]
        public int CategoryId { get; set; }

        // Accept the file upload from the form here
        public IFormFile? NewProductImg { get; set; }
    }

    public class UpdateProductHandler
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Inject IWebHostEnvironment so we can save files to wwwroot
        public UpdateProductHandler(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ProductDto?> HandleAsync(UpdateProductCommand command)
        {
            var product = await _context.Products.FindAsync(command.Id);

            if (product == null)
                return null;

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == command.CategoryId);
            if (!categoryExists)
                throw new Exception("The specified CategoryId does not exist.");

            // 1. Process the image file if a new one was uploaded
            if (command.NewProductImg != null && command.NewProductImg.Length > 0)
            {
                // Optional: Delete the old physical file here if you want to clean up storage

                // Create a unique file name
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                // Ensure the directory exists
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + command.NewProductImg.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file to disk
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await command.NewProductImg.CopyToAsync(fileStream);
                }

                // Update the product's database record with the new URL path
                product.ProductImg = "/images/" + uniqueFileName;
            }
            // NOTE: If command.NewProductImg is null, we do nothing, 
            // which leaves the existing product.ProductImg untouched!

            // 2. Update the rest of the fields
            product.Name = command.Name;
            product.Description = command.Description;
            product.Price = command.Price;
            product.IsAvailable = command.IsAvailable;
            product.CategoryId = command.CategoryId;

            await _context.SaveChangesAsync();

            // 3. Map to DTO
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                ProductImg = product.ProductImg, // This will contain either the old URL or the brand new one
                Description = product.Description,
                Price = product.Price,
                IsAvailable = product.IsAvailable,
                CategoryId = product.CategoryId
            };
        }
    }
}