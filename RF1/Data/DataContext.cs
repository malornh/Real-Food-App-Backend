using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RF1.Dtos;
using RF1.Models;
using System.Reflection.Emit;

namespace RF1.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Farm> Farms { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<PhotoLink> PhotoLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Farm>()
                .HasOne(f => f.Photo)
                .WithMany()
                .HasForeignKey(f => f.PhotoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Farm>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
