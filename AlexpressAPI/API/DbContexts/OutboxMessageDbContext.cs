using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.DbContexts
{
    public class OutboxMessageDbContext : DbContext
    {
        public OutboxMessageDbContext(DbContextOptions<OutboxMessageDbContext> options) : base(options) { }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }
    }
}
