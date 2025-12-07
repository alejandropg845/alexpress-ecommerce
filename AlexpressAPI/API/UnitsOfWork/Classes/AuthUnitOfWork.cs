using API.DbContexts;
using API.Interfaces.Repositories;
using API.Repositories;
using API.UnitsOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace API.UnitsOfWork.Classes
{
    public class AuthUnitOfWork : IDisposable, IAuthUnitOfWork
    {
        private readonly AuthDbContext _authDbContext;
        private readonly OutboxMessageDbContext _outboxDbContext;
        private readonly RTokenDbContext _rTokenDbContext;

        public IOutboxMessageRepository OutboxMessageRepository { get; private set; }
        public IUsersRepository UserRepository { get; private set; }
        public IRTokenRepository RTokenRepository { get; private set; }
        public AuthUnitOfWork(AuthDbContext authDbContext, OutboxMessageDbContext outboxDbContext, RTokenDbContext rTokenDbContext)
        {
            _authDbContext = authDbContext;
            _outboxDbContext = outboxDbContext;
            _rTokenDbContext = rTokenDbContext;

            OutboxMessageRepository = new OutBoxMessageRepository(outboxDbContext);
            UserRepository = new UserRepository(authDbContext);
            RTokenRepository = new RTokenRepository(rTokenDbContext);
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            var transaction = await _authDbContext.Database.BeginTransactionAsync();

            await _outboxDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

            return transaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        => await transaction.CommitAsync();

        public async Task RollbackTransactionAsync(IDbContextTransaction transaction)
        => await transaction.RollbackAsync();

        public async Task SaveContextsChangesAsync()
        {
            await _authDbContext.SaveChangesAsync();
            await _outboxDbContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            _authDbContext.Dispose();
            _outboxDbContext.Dispose();
        }
    }
}
