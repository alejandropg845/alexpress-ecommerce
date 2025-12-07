
using API.Interfaces.Coupon;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.CouponDTO
{
    public class CreateCouponDTO : ICouponDto
    {
        public string? CouponName { get; set; }
        [Range(0, 100)] public decimal Discount { get; set; }
    }   
}
