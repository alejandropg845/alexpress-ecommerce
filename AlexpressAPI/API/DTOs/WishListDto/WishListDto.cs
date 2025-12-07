using API.Entities;

namespace API.DTOs.WishListDto
{
    public class WishListDto
    {
        public string AppUserId { get; set; } = string.Empty;
        public List<WishlistItemDto> WishListProducts { get; set; }
    }
}
