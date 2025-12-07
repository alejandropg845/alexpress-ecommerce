

namespace API.DTOs.OrderDTO
{
    public class OrderedProductDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public decimal ShippingPrice { get; set; }
        public string? CouponName { get; set; }
    }
}
