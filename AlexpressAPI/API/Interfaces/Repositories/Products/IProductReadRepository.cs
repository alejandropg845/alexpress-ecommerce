using API.DTOs.CartProductsDTO;
using API.DTOs.ProductDto;

namespace API.Interfaces.Repositories.Products
{
    public interface IProductReadRepository
    {
        Task<List<ToProductThumbnailDto>> GetUserProductsAsync(string userId);
        Task<List<ToProductThumbnailDto>> GetProductsAsync(string? title, int categoryId, string? userId, decimal price);
        Task<ToProductDto?> GetProductDtoAsync(int id);
        Task<ToProductDto?> GetProductDtoToUpdateAsync(int id, string appUserId);
        Task<bool> ProductExistsAsync(int id);
        Task<(bool IsStock, string? ErrorMessage)> IsStockAvailableAsync(List<CartProductDto> orderedProducts);

    }
}
