using API.DTOs.ProductDto;
using API.Entities;
using API.Responses.Product;

namespace API.Interfaces.Services
{
    public interface IProductService
    {
        Task<List<ToProductThumbnailDto>> GetUserProductsAsync(string userId);
        Task<List<ToProductThumbnailDto>> GetProductsAsync(string? title, int categoryId, string? userId, decimal price);
        Task<ToProductDto?> GetProductAsync(int id);
        Task<ToProductDto?> GetProductDtoToUpdateAsync(int id, string appUserId);
        Task<CreateProductResponse> CreateProductAsync(CreateProductDto product, string userId, string username);
        Task<UpdateProductResponse> UpdateProductAsync(int id, UpdateProductDto dto, string userId);
        Task<DeleteProductResponse> DeleteProductAsync(int id, string userId);
        Task DeleteFromCloudinaryAsync(string publicId);
    }
}
