using API.Entities;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.DbContexts
{
    public class WishListDbContext : DbContext
    {
        public WishListDbContext(DbContextOptions<WishListDbContext> options) : base(options) { }

        public DbSet<WishList> WishList { get; set; }
        public DbSet<WishListProduct> WishListItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FakeUser>().ToTable("AspNetUsers", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Product>().ToTable("Products", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Condition>().ToTable("Conditions", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Category>().ToTable("Categories", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Coupon>().ToTable("Coupons", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<ReviewItem>().ToTable("ReviewItems", t => t.ExcludeFromMigrations());

            modelBuilder.Entity<WishList>().HasOne<FakeUser>()
                                            .WithOne()
                                            .HasForeignKey<WishList>(w => w.AppUserId)
                                            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<WishList>().HasMany(c => c.WishListProducts)
                                      .WithOne(c => c.WishList)
                                      .HasForeignKey(c => c.WishListId)
                                      .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WishListProduct>().HasOne(c => c.Product)
                                              .WithMany()
                                              .HasForeignKey(c => c.ProductId)
                                              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
