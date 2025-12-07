

using API.DTOs.CouponDTO;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.ProductDto
{
    public class UpdateProductDto
    {
        [Required]
        [MinLength(4, ErrorMessage = "Number of images must be 4")]
        [MaxLength(4, ErrorMessage = "Number of images must be 4")]
        public List<string> Images { get; set; }

        [Required]
        [MaxLength(160, ErrorMessage = "Title exceeds max length")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000, ErrorMessage = "Description exceeds max length")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, 10000, ErrorMessage = "Price exceeds max value (10000)")]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int ConditionId { get; set; }

        [Required]
        public decimal ShippingPrice { get; set; }
        public UpdateCouponDto Coupon { get; set; }
        [Range(1, 1000000)] public int Stock { get; set; }
    }
}
