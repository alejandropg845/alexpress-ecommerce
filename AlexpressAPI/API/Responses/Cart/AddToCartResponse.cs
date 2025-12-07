using API.Entities;

namespace API.Responses.Cart
{
    public class AddToCartResponse
    {
        public bool NoMoreStock {  get; set; }
        public bool TwoOrMoreUnitsRequired { get; set; }
        public bool ProductExists { get; set; }
        public bool OwnProduct {  get; set; }
        public decimal Summary { get; set; }
        public bool IsProductRemoved { get; set; }
        public CartProduct CartProduct { get; set; }
        public int CartId { get; set; }
    }
}
