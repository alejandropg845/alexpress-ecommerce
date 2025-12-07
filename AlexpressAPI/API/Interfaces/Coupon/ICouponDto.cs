namespace API.Interfaces.Coupon
{
    public interface ICouponDto
    {
        string? CouponName { get; set; }
        decimal Discount { get; set; }
    }
}
