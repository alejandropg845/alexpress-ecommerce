using API.Entities;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.DbContexts
{
    public class AddressDbContext : DbContext
    {
        public AddressDbContext(DbContextOptions<AddressDbContext> options) : base (options) { }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FakeUser>().ToTable("AspNetUsers", t => t.ExcludeFromMigrations());

            modelBuilder.Entity<Address>()
                .HasOne<FakeUser>()
                .WithMany()
                .HasForeignKey(a => a.AppUserId);
        }
    }
}
