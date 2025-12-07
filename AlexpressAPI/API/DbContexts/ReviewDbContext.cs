using API.Entities;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.DbContexts
{
    public class ReviewDbContext : DbContext
    {
        public ReviewDbContext(DbContextOptions<ReviewDbContext> options) : base(options) { }
        public DbSet<ReviewItem> ReviewItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().ToTable("Products", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Condition>().ToTable("Conditions", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Category>().ToTable("Categories", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Coupon>().ToTable("Coupons", t => t.ExcludeFromMigrations());


        }
    }
}
