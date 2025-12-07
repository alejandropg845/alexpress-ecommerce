using API.DTOs.ProductDto;
using API.Entities;

namespace API.Responses.Product
{
    public class CreateProductResponse
    {
        public ToProductDto ProductDto { get; set; }
        public bool IsExplicitImage { get; set; }
        public bool IsExplicitDescription { get; set; }
        public bool UserExists { get; set; }
        public bool IsExplicitTitle {  get; set; }
    }
}
