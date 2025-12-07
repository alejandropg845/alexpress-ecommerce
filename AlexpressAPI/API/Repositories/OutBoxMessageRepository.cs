using API.DbContexts;
using API.Entities;
using API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class OutBoxMessageRepository : IOutboxMessageRepository
    {
        private readonly OutboxMessageDbContext _context;
        public OutBoxMessageRepository(OutboxMessageDbContext context)
        {
            _context = context;
        }
        public void SaveOutboxMessage(OutboxMessage outboxMessage) 
            => _context.OutboxMessages.Add(outboxMessage);

        public async Task<List<OutboxMessage>> GetOutboxMessagesAsync()
        {
            return await _context.OutboxMessages
            .Where(om => !om.IsProcessed)
            .OrderBy(om => om.Id)
            .Take(5)
            .ToListAsync();
        }
        public async Task DeleteOutboxMessageAsync(OutboxMessage outboxMessage)
        {
            _context.OutboxMessages.Remove(outboxMessage);
            await _context.SaveChangesAsync();
        }
        public async Task SaveContextChangesAsync() => await _context.SaveChangesAsync();
    }
}
