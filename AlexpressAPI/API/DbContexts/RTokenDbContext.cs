using API.Entities;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API.DbContexts
{
    public class RTokenDbContext : DbContext
    {
        public RTokenDbContext(DbContextOptions<RTokenDbContext> options) : base (options) { }

        public DbSet<RToken> RTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FakeUser>().ToTable("AspNetUsers", u => u.ExcludeFromMigrations());

            modelBuilder.Entity<RToken>().HasOne<FakeUser>()
                                        .WithMany()
                                        .HasForeignKey(t => t.AppUserId)
                                        .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
