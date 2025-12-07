using API.Entities;
using API.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace API.DbContexts
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base (options) { }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartProduct> CartProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FakeUser>().ToTable("AspNetUsers", t => t.ExcludeFromMigrations());
            builder.Entity<Product>().ToTable("Products", t => t.ExcludeFromMigrations());
            builder.Entity<Condition>().ToTable("Conditions", t => t.ExcludeFromMigrations());
            builder.Entity<Category>().ToTable("Categories", t => t.ExcludeFromMigrations());
            builder.Entity<Coupon>().ToTable("Coupons", t => t.ExcludeFromMigrations());
            builder.Entity<ReviewItem>().ToTable("ReviewItems", t => t.ExcludeFromMigrations());

            builder.Entity<Cart>()
                .HasOne<FakeUser>()
                .WithOne()
                .HasForeignKey<Cart>(a => a.AppUserId);

            builder.Entity<Cart>().HasMany(c => c.CartProducts)
                                  .WithOne()
                                  .HasForeignKey(c => c.CartId)
                                  .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<CartProduct>().HasOne(c => c.Product)
                                            .WithMany()
                                          .HasForeignKey(c => c.ProductId)
                                          .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
