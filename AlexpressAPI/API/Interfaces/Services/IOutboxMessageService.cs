using API.Entities;

namespace API.Interfaces.Services
{
    public interface IOutboxMessageService
    {
        Task ProcessOutboxMessagesAsync();
    }
}
