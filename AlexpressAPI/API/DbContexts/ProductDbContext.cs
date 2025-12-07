using API.Entities;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.DbContexts
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base (options){}
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FakeUser>().ToTable("AspNetUsers", t => t.ExcludeFromMigrations());
            builder.Entity<ReviewItem>().ToTable("ReviewItems", t => t.ExcludeFromMigrations());

            builder.Entity<Product>().HasOne<FakeUser>()
                                      .WithMany()
                                      .HasForeignKey(p => p.AppUserId)
                                      .OnDelete(DeleteBehavior.NoAction); // <== Nunca se va a eliminar un Product

            builder.Entity<Coupon>().HasOne(p => p.Product)
                                     .WithOne(p => p.Coupon)
                                     .HasForeignKey<Coupon>(p => p.ProductId)
                                     .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "watches" },
                new Category { Id = 2, Name = "clothes" },
                new Category { Id = 3, Name = "shoes" },
                new Category { Id = 4, Name = "pet toys" },
                new Category { Id = 5, Name = "phones & tablets" },
                new Category { Id = 6, Name = "TV & home appliances" },
                new Category { Id = 7, Name = "audio & sound" }
            );

            builder.Entity<Condition>().HasData(
                new Condition { Id = 1, Name = "used" },
                new Condition { Id = 2, Name = "refurbished" },
                new Condition { Id = 3, Name = "new" }
            );
        }
    }
}
