using API.DTOs.CartProductsDTO;

namespace API.DTOs.CartDTO
{
    public class ToCartDto
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public decimal Summary { get; set; }
        public List<CartProductDto> CartProducts { get; set; } = [];

    }
}
