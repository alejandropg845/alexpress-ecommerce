using API.DTOs.CartDTO;
using API.DTOs.ProductDto;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs.CartProductsDTO
{
    public class CartProductDto
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public string? CouponName { get; set; }
        public decimal CustomizedDiscount { get; set; }
        public decimal Price { get; set; }
        public decimal NewPrice { get; set; }
        public decimal ShippingPrice { get; set; }
    }
}
