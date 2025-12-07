using API.Entities;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.DbContexts
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderedProduct> OrderedProducts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FakeUser>().ToTable("AspNetUsers", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Address>().ToTable("Addresses", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Product>().ToTable("Products", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Condition>().ToTable("Conditions", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Category>().ToTable("Categories", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<Coupon>().ToTable("Coupons", t => t.ExcludeFromMigrations());
            modelBuilder.Entity<ReviewItem>().ToTable("ReviewItems", t => t.ExcludeFromMigrations());
            

            modelBuilder.Entity<Order>().HasOne<FakeUser>()
                                        .WithMany()
                                        .HasForeignKey(o => o.AppUserId)
                                        .OnDelete(DeleteBehavior.NoAction);

            //Order can have many OrderedProducts.
            modelBuilder.Entity<Order>().HasMany(o => o.OrderedProducts)
                                   .WithOne()
                                   .HasForeignKey(p => p.OrderId)
                                   .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<OrderedProduct>().HasOne(p => p.Product)
                                            .WithMany()
                                            .HasForeignKey(p => p.ProductId)
                                            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>().HasOne(o => o.Address)
                                        .WithMany()
                                        .HasForeignKey(o => o.AddressId);
        }
    }
}
