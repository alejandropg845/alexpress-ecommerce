namespace API.Responses.Cart
{
    public class RemoveCartProductResponse
    {
        public bool CartProductExists { get; set; }
        public bool UserExists { get; set; }
        public decimal Summary { get; set; }
    }
}
