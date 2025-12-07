using API.DbContexts;
using API.DTOs.CartDTO;
using API.DTOs.CartProductsDTO;
using API.Entities;
using API.Interfaces.Repositories;
using API.Interfaces.Repositories.Products;
using API.Interfaces.Services;
using API.Repositories;
using API.Services.App;
using API.UnitsOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.UnitsOfWork.Classes
{
    public class OrderUnitOfWork : IOrderUnitOfWork, IDisposable
    {
        private readonly OrderDbContext _orderDbContext;
        private readonly CartDbContext _cartDbContext;
        private readonly ProductDbContext _productDbContext;
        private readonly AuthDbContext _authDbContext;
        private readonly AddressDbContext _addressDbContext;
        private readonly OutboxMessageDbContext _outboxMessageDbContext;
        private readonly ReviewDbContext _reviewDbContext;

        public IOrderRepository OrderRepository { get; private set; }
        public IProductWriteRepository ProductWriteRepository { get; private set; }
        public IProductReadRepository ProductReadRepository { get; private set; }
        public ICartRepository CartRepository {  get; private set; }
        public IUsersRepository UsersRepository {  get; private set; }
        public IAddressRepository AddressRepository { get; private set; }
        public IOutboxMessageRepository OutboxMessageRepository { get; private set; }
        public IReviewRepository ReviewRepository { get; private set; }
        public OrderUnitOfWork(CartDbContext cartDbContext, 
            ProductDbContext productDbContext, 
            OrderDbContext orderDbContext, 
            AuthDbContext authDbContext, 
            AddressDbContext addressDbContext, 
            OutboxMessageDbContext outboxMessageDbContext,
            ReviewDbContext reviewDbContext
            )
        {
            _orderDbContext = orderDbContext;
            _cartDbContext = cartDbContext;
            _productDbContext = productDbContext;
            _authDbContext = authDbContext;
            _addressDbContext = addressDbContext;
            _outboxMessageDbContext = outboxMessageDbContext;
            _reviewDbContext = reviewDbContext;

            OrderRepository = new OrderRepository(_orderDbContext);
            ProductWriteRepository = new ProductRepository(_productDbContext);
            ProductReadRepository = new ProductRepository(_productDbContext);
            CartRepository = new CartRepository(_cartDbContext);
            UsersRepository = new UserRepository(_authDbContext);
            AddressRepository = new AddressRepository(_addressDbContext);
            OutboxMessageRepository = new OutBoxMessageRepository(_outboxMessageDbContext);
            ReviewRepository = new ReviewRepository(_reviewDbContext);
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            var transaction = await _productDbContext.Database.BeginTransactionAsync();

            await _cartDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());
            await _orderDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());
            await _outboxMessageDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());
            await _reviewDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

            return transaction;
        }
        public async Task SaveAllChangesAsync()
        {
            await _cartDbContext.SaveChangesAsync();
            await _orderDbContext.SaveChangesAsync();
            await _productDbContext.SaveChangesAsync();
            await _outboxMessageDbContext.SaveChangesAsync();
            await _reviewDbContext.SaveChangesAsync();
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        => await transaction.CommitAsync();
        public async Task RollbackTransactionAsync(IDbContextTransaction transaction)
        => await transaction.RollbackAsync();
        
        public void Dispose()
        {
            _orderDbContext.Dispose();
            _cartDbContext.Dispose();
            _productDbContext.Dispose();
            _authDbContext.Dispose();
        }

    }
}
