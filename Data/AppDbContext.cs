using Microsoft.EntityFrameworkCore;
using aps.net_order_system.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Import your Models namespace

namespace aps.net_order_system.Data
{
    public class AppDbContext : IdentityDbContext<UserModel>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // This creates the "Users" table in your database
        public DbSet<CategoriesModel> Categories { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<OrderItemModel> OrderItems { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<TotalCountOderModel> TotalCountOrders { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: Ensure Email or Username is Unique
            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<PaymentModel>()
            .Property(p => p.Amount)
            .HasColumnType("decimal(18,2)");
        }

    }
}