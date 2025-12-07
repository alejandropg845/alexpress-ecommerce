using API.Entities;
using API.Interfaces.Coupon;

namespace API.Features.Cart.Coupons
{
    public class Is50OffOneProduct : ICouponStrategy
    {
        public decimal CalculatePrice(decimal price, int quantity, decimal shippingPrice)
        {
            return (price * (quantity - 1) + (decimal)0.5 * price) + shippingPrice;
        }
    }
}
