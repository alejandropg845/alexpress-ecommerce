using API.DTOs.ProductDto;

namespace API.Responses.Product
{
    public class UpdateProductResponse
    {
        public ToProductDto ProductDto { get; set; }
        public bool IsExplicitContent { get; set; }
        public bool ProductExists { get; set; }
        public bool UserExists { get; set; }
    }
}
