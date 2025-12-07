using API.DTOs.CartDTO;
using API.DTOs.CartProductsDTO;
using API.Responses.Cart;

namespace API.Interfaces.Services
{
    public interface ICartService
    {
        Task<ToCartDto?> GetCartAsync(string userId);
        Task<AddToCartResponse> AddToCartAsync(string userId, AddToCartDto dto);
        Task<RemoveCartProductResponse> RemoveCartProductAsync(int productId, string userId);

    }
}
