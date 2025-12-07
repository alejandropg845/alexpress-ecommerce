
using API.DTOs.CouponDTO;
using API.DTOs.ReviewDTO;


namespace API.DTOs.ProductDto
{
    public class ToProductDto
    {
        public int Id { get; set; } 
        public string AppUserId { get; set; }
        public string Username { get; set; }
        public List<string> Images { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int Votes { get; set; }
        public int Accumulated { get; set; }
        public decimal Price { get; set; }
        public int ConditionId { get; set; }
        public string Condition { get; set; }
        public decimal ShippingPrice { get; set; }
        public int Stock { get; set; }
        public int Sold { get; set; }
        public CouponDto Coupon { get; set; }
        public List<ReviewItemDto> Reviews { get; set; }
        public bool IsDeleted { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
