using API.DbContexts;
using API.DTOs.CouponDTO;
using API.DTOs.ProductDto;
using API.DTOs.ReviewDTO;
using API.DTOs.UsersDto;
using API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ProductDbContext _productContext;
        private readonly AuthDbContext _authContext;
        public AdminRepository(IConfiguration config, AuthDbContext authDb, ProductDbContext productDb)
        {
            _productContext = productDb;
            _authContext = authDb;
        }

        public async Task<List<ToProductThumbnailDto>> GetAllProductsAsync()
        {
            return await _productContext.Products
                .Select(p => new ToProductThumbnailDto
                {
                    AppUserId = p.AppUserId,
                    Username = p.Username,
                    Title = p.Title,
                    Price = p.Price,
                    Category = p.Category.Name,
                    Condition = p.Condition.Name,
                    ShippingPrice = p.ShippingPrice,
                    Image = p.Images[0],
                    Stock = p.Stock,
                    Sold = p.Sold,
                    Accumulated = p.Accumulated,
                    Votes = p.Votes,
                    Id = p.Id
                }).ToListAsync();
        }
        public async Task<List<UserDto>> GetUsersAsync()
        {
            return await _authContext
                .Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    IsDisabled = u.IsDisabled,
                    UserName = u.UserName!
                })
                .ToListAsync();
        }

        public async Task DeleteProductAsync(int productId)
        {
            await _productContext.Products
                .ExecuteUpdateAsync(p => p.SetProperty(p => p.IsDeleted, true));
        }
        public async Task DisableUserAsync(string userId)
        {
            await _authContext.Users
               .ExecuteUpdateAsync(u => u.SetProperty(u => u.IsDisabled, true));
        }
        
    }
}
