using API.Interfaces.Coupon;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.CouponDTO
{
    public class UpdateCouponDto : ICouponDto
    {
        [Range(0, 100)] public decimal Discount { get; set; }
        public string? CouponName { get; set; }
    }
}
