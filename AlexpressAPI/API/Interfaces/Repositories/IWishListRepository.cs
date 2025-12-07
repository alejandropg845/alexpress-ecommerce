
using API.DTOs.WishListDto;
using API.Entities;

namespace API.Interfaces.Repositories
{
    public interface IWishListRepository
    {
        Task<WishListDto?> GetWishListDtoAsync(string userId);
        Task<WishList?> GetWishlistAsync(string userId);
        Task<WishlistItemDto?> GetWishlistItemAsync(int wishlistId, int productId);
        void AddWishlist(WishList wishlist);
        Task<bool> IsProductInWishlistAsync(int wishlistId, int productId);
        Task SaveContextChangesAsync();
        Task RemoveFromWishlistAsync(int wishlistId, int productId);
    }
}
