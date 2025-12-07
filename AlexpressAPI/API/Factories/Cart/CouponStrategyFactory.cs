using API.Features.Cart.Coupons;
using API.Interfaces.Coupon;
using API.Interfaces.Factories;

namespace API.Factories.Cart
{
    public class CouponStrategyFactory : ICouponStrategyFactory
    {
        private readonly Dictionary<string, ICouponStrategy> _strategies;
        public CouponStrategyFactory()
        {
            _strategies = new Dictionary<string, ICouponStrategy>
            {
                { "is50OffOneProduct", new Is50OffOneProduct() },
                { "is50Discount", new Is50Discount() },
                { "isFreeShipping", new IsFreeShipping() }
            };
        }
        public ICouponStrategy GetStrategy(string? couponName)
        {
            if (couponName == null)

                return new NoCoupon();

            var strategy = _strategies[couponName];

            return strategy ?? throw new Exception("This coupon is not valid");
        }
    }
}
