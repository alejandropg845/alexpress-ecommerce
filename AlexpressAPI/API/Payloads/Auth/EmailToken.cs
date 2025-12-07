
using System.Text.Json.Serialization;

namespace API.Payloads.Auth
{
    public class EmailToken 
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
