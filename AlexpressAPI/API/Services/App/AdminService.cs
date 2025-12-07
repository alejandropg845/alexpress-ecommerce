using API.DTOs.ProductDto;
using API.DTOs.UsersDto;
using API.Entities;
using API.Interfaces.Repositories;
using API.Interfaces.Services;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace API.Services.App
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        public AdminService(IAdminRepository repo)
        {
            _adminRepository = repo;
        }
        public async Task<List<ToProductThumbnailDto>> GetAllProductsAsync()
        => await _adminRepository.GetAllProductsAsync();
        public async Task<List<UserDto>> GetUsersAsync()
        => await _adminRepository.GetUsersAsync();
        public async Task DeleteProductAsync(int productId)
        => await _adminRepository.DeleteProductAsync(productId);
        public async Task DisableUserAsync(string userId)
        => await _adminRepository.DisableUserAsync(userId);
    }
}
