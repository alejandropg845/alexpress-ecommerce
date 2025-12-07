namespace API.Responses.Order
{
    public class SummarizeOrderResponse
    {
        public string SessionUrl { get; set; }
        public bool IsCart { get; set; }
        public bool UserExists { get; set; }
        public bool AddressExists { get; set; }
    }
}
