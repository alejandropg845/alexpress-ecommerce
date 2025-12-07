using API.DTOs.WishListDto;
using API.Entities;
using API.Interfaces.Repositories;
using API.Interfaces.Repositories.Products;
using API.Interfaces.Services;
using API.Responses.Wishlist;

namespace API.Services.App
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishListRepository _wishListRepository;
        private readonly IProductWriteRepository _productRepository;
        private readonly IProductReadRepository _productReadRepository;
        public WishlistService(IWishListRepository r, IProductWriteRepository productRepository, IProductReadRepository productReadRepository)
        {
            _wishListRepository = r;
            _productRepository = productRepository;
            _productReadRepository = productReadRepository;
        }
        public async Task<WishListDto?> GetWishlistAsync(string userId)
        => await _wishListRepository.GetWishListDtoAsync(userId);
        public async Task<WishlistItemDto?> GetWishlistItemAsync(int wishlistId, int productId)
        => await _wishListRepository.GetWishlistItemAsync(wishlistId, productId);
        public async Task<AddToWishlistResponse> AddToWishListAsync(string userId, int productId)
        {
            var response = new AddToWishlistResponse();

            var product = await _productReadRepository.GetProductDtoAsync(productId);

            response.ProductExists = product is not null;
            if (product == null) return response;

            response.IsUserProduct = product.AppUserId == userId;

            if (response.IsUserProduct) return response;

            var wishlist = await _wishListRepository.GetWishlistAsync(userId);
            
            if (wishlist == null)
            {
                wishlist = new WishList { AppUserId = userId };

                _wishListRepository.AddWishlist(wishlist);
            }

            if (wishlist.Id is not 0) // <== Si no es 0 el Id, entonces ya existía el Wishlist
            {
                var isProductInWishlist = await _wishListRepository
                    .IsProductInWishlistAsync(wishlist.Id, productId);

                response.IsProductInWishlist = isProductInWishlist;

                if (isProductInWishlist) return response;

                AddWishlistProduct(productId, wishlist);

            }
            else AddWishlistProduct(productId, wishlist);

            await _wishListRepository.SaveContextChangesAsync();

            var addedWishlistItem = await GetWishlistItemAsync(wishlist.Id, productId);

            response.AddedWishlistItem = addedWishlistItem!;

            return response;
        }

        private static void AddWishlistProduct(int productId, WishList wishlist)
        {
            var newWishlistProduct = new WishListProduct
            {
                ProductId = productId,
                WishListId = wishlist.Id
            };

            wishlist.WishListProducts.Add(newWishlistProduct);
        }

        public async Task DeleteFromWishlistAsync(string userId, int productId)
        {
            var wishlist = await _wishListRepository.GetWishlistAsync(userId);

            await _wishListRepository.RemoveFromWishlistAsync(wishlist!.Id, productId);
        }
    }
}
