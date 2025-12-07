using API.DbContexts;
using API.DTOs.CartDTO;
using API.DTOs.CartProductsDTO;
using API.Entities;
using API.Interfaces.Repositories;
using API.Interfaces.Repositories.Products;
using API.Interfaces.Services;
using API.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.UnitsOfWork.Interfaces
{
    public interface IOrderUnitOfWork
    {
        public IOrderRepository OrderRepository { get;}
        public IProductWriteRepository ProductWriteRepository { get; }
        public IProductReadRepository ProductReadRepository { get; }
        public ICartRepository CartRepository { get; }
        public IUsersRepository UsersRepository { get; }
        public IAddressRepository AddressRepository { get; }
        public IOutboxMessageRepository OutboxMessageRepository { get; }
        public IReviewRepository ReviewRepository { get; }
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync(IDbContextTransaction transaction);
        Task RollbackTransactionAsync(IDbContextTransaction transaction);
        Task SaveAllChangesAsync();

    }
}
