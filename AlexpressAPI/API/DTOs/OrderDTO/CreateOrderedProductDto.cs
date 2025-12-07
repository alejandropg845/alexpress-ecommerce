
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs.OrderDTO
{
    public class CreateOrderedProductDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public decimal ShippingPrice { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public string? CouponName { get; set; } = string.Empty;
        public decimal? DiscountValue { get; set; }
    }
}
