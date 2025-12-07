namespace API.DTOs.ProductDto
{
    public class ToProductThumbnailDto
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Condition { get; set; }
        public decimal ShippingPrice { get; set; }
        public string Image {  get; set; }
        public int Stock {  get; set; }
        public int CategoryId { get; set; }
        public string Username { get; set; }
        public int Accumulated { get; set; }
        public int Sold { get; set; }
        public int Votes { get; set; }
    }
}
