using API.DTOs.WishListDto;

namespace API.Responses.Wishlist
{
    public class AddToWishlistResponse
    {
        public bool ProductExists { get; set; }
        public bool IsUserProduct { get; set; }
        public bool IsProductInWishlist {  get; set; }
        public WishlistItemDto AddedWishlistItem { get; set; }
    }
}
