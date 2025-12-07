
using API.DTOs.ProductDto;
using API.DTOs.UsersDto;

namespace API.Interfaces.Repositories
{
    public interface IAdminRepository
    {
        Task<List<ToProductThumbnailDto>> GetAllProductsAsync();
        Task<List<UserDto>> GetUsersAsync();
        Task DeleteProductAsync(int productId);
        Task DisableUserAsync(string userId);

    }
}
