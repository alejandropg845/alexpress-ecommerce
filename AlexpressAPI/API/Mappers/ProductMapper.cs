using API.DTOs.ProductDto;
using API.Entities;

namespace API.Mappers
{
    public static class ProductMapper
    {
        public static Product ToProduct(this CreateProductDto dto, string userId, string username)
        {
            return new Product
            {
                CategoryId = dto.CategoryId,
                Accumulated = 0,
                AppUserId = userId,
                ConditionId = dto.ConditionId,
                Coupon = new Coupon
                {
                    CouponName = dto.Coupon.CouponName,
                    Discount = dto.Coupon.Discount,
                },
                Description = dto.Description,
                Images = dto.Images,
                Price = dto.Price,
                ShippingPrice = dto.ShippingPrice,
                Title = dto.Title,
                Stock = dto.Stock,
                Username = username
            };
        }
    }
}
