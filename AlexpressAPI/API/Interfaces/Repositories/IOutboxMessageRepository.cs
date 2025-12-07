using API.Entities;

namespace API.Interfaces.Repositories
{
    public interface IOutboxMessageRepository
    {
        void SaveOutboxMessage(OutboxMessage outboxMessage);
        Task<List<OutboxMessage>> GetOutboxMessagesAsync();
        Task DeleteOutboxMessageAsync(OutboxMessage outboxMessage);
        Task SaveContextChangesAsync();

    }
}
