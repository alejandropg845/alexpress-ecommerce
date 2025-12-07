using System.ComponentModel.DataAnnotations;

namespace API.DTOs.CartProductsDTO
{
    public class AddToCartDto
    {
        [Required] public int ProductId { get; set; }
        [Range(-1, 10)] public int Quantity { get; set; }
        public int CustomizedDiscount { get; set; }
        public string? CouponName { get; set; }

    }
}
