
using API.Entities;

namespace API.Responses.Order
{
    public class CreateOrderResponse
    {
        public bool UserExists { get; set; }
        public bool CartIsNull { get; set; }
        public bool IsStockAvailable { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
