using API.Interfaces.Coupon;

namespace API.Features.Cart.Coupons
{
    public class NoCoupon : ICouponStrategy
    {
        public decimal CalculatePrice(decimal price, int quantity, decimal shippingPrice)
        {
            return price * quantity + shippingPrice;
        }
    }
}
