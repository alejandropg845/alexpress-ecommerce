using API.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTOs.CouponDTO
{
    public class CouponDto
    {
        public string? CouponName { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Discount { get; set; }
    }
}
