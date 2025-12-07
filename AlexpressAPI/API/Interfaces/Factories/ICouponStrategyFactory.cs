using API.Interfaces.Coupon;

namespace API.Interfaces.Factories
{
    public interface ICouponStrategyFactory
    {
        ICouponStrategy GetStrategy(string? couponName);
    }
}
