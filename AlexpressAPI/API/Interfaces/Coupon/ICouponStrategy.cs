namespace API.Interfaces.Coupon
{
    public interface ICouponStrategy
    {
        decimal CalculatePrice(decimal price, int quantity, decimal shippingPrice);
    }
}
