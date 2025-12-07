
using API.DTOs.OrderDTO;
using System.Text.Json.Serialization;

namespace API.Payloads.Order
{
    public record OrderMail
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }


        [JsonPropertyName("username")]
        public string Username { get; set; }


        [JsonPropertyName("orderedProducts")]
        public List<OrderedProductDto> OrderedProducts { get; set; }

        [JsonPropertyName("summary")]
        public decimal Summary {  get; set; }
    }
}
