using API.DTOs.WishListDto;
using API.Responses.Wishlist;

namespace API.Interfaces.Services
{
    public interface IWishlistService
    {
        Task<WishListDto?> GetWishlistAsync(string userId);
        Task<AddToWishlistResponse> AddToWishListAsync(string userId, int productId);
        Task DeleteFromWishlistAsync(string userId, int productId);
        Task<WishlistItemDto?> GetWishlistItemAsync(int wishlistId, int productId);

    }
}
