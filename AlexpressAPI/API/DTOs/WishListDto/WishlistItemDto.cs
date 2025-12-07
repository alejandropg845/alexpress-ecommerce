namespace API.DTOs.WishListDto
{
    public class WishlistItemDto
    {
        public int WishListId { get; set; }
        public int ProductId { get; set; }
        public string Image { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
    }
}
