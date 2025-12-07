using API.Entities;
using API.Interfaces.Coupon;

namespace API.Features.Cart.Coupons
{
    public class Is50Discount : ICouponStrategy
    {
        public decimal CalculatePrice(decimal price, int quantity, decimal shippingPrice)
        {
            return (price * quantity * (decimal)0.5) + shippingPrice ;
        }
    }
}
