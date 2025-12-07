using API.DTOs.ProductDto;
using API.DTOs.UsersDto;

namespace API.Interfaces.Services
{
    public interface IAdminService
    {
        Task<List<ToProductThumbnailDto>> GetAllProductsAsync();
        Task<List<UserDto>> GetUsersAsync();
        Task DeleteProductAsync(int productId);
        Task DisableUserAsync(string userId);

    }
}
