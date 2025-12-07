using API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.UnitsOfWork.Interfaces
{
    public interface IAuthUnitOfWork
    {
        IOutboxMessageRepository OutboxMessageRepository { get; }
        IUsersRepository UserRepository { get;}
        IRTokenRepository RTokenRepository { get; }
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync(IDbContextTransaction transaction);
        Task RollbackTransactionAsync(IDbContextTransaction transaction);
        Task SaveContextsChangesAsync();

    }
}
