using API.DbContexts;
using API.DTOs.WishListDto;
using API.Entities;
using API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class WishListRepository : IWishListRepository
    {
        private readonly WishListDbContext _context;
        public WishListRepository(WishListDbContext context)
        {
            _context = context;
        }
        public async Task<WishListDto?> GetWishListDtoAsync(string userId)
        {
            var wishList = await _context.WishList
                .Where(w => w.AppUserId == userId)
                .Select(wl => new WishListDto
                {
                    WishListProducts = wl.WishListProducts.Select(wli => new WishlistItemDto
                    {
                        ProductId = wli.ProductId,
                        Image = wli.Product.Images[0],
                        Title = wli.Product.Title
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return wishList;
        }
        public async Task<WishList?> GetWishlistAsync(string userId)
        {
            var wishlist = await _context.WishList.FirstOrDefaultAsync(wl => wl.AppUserId == userId);

            return wishlist;
        }
        public void AddWishlist(WishList wishlist) => _context.WishList.Add(wishlist);

        public async Task<bool> IsProductInWishlistAsync(int wishlistId, int productId)
        => await _context.WishListItems.AnyAsync(wli => wli.WishListId == wishlistId && wli.ProductId == productId);
        public async Task<WishlistItemDto?> GetWishlistItemAsync(int wishlistId, int productId)
        {
            var wishlistItem = await _context.WishListItems
                .Where(wlp => wlp.WishListId == wishlistId
                    && wlp.ProductId == productId)
                .Select(wlp => new WishlistItemDto
                {
                    ProductId = wlp.ProductId,
                    Image = wlp.Product.Images[0],
                    Title = wlp.Product.Title,
                    Username = wlp.Product.Username
                })
                .FirstOrDefaultAsync();
        
            return wishlistItem;
        }

        public async Task SaveContextChangesAsync() => await _context.SaveChangesAsync();
        public async Task RemoveFromWishlistAsync(int wishlistId, int productId)
        {
            await _context.WishListItems
            .Where(
                wlp => wlp.WishListId == wishlistId 
                && wlp.ProductId == productId
            )
            .ExecuteDeleteAsync();

        }

    }
}
