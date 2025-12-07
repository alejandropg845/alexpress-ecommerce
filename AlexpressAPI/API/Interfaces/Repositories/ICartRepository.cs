using API.DTOs.CartDTO;
using API.DTOs.OrderDTO;
using API.Entities;

namespace API.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<ToCartDto?> GetCartAsync(string userId);
        Task<Cart?> GetCartIfExistsAsync(string userId);
        Task<CartProduct?> GetProductInCartIfExistsAsync(int cartId, int productId);
        void AddCart(Cart cart);
        Task<ToCartDto?> GetOrderedCartDtoAsync(string userId);
        Task<bool> CartExistsAndContainsProductsAsync(string userId);
        Task EmptyUserCartAsync(int cartId);
        Task SaveContextChangesAsync();
    }
}
