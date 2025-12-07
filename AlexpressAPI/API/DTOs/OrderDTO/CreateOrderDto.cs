using API.DTOs.CartProductsDTO;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.OrderDTO
{
    public class CreateOrderDto
    {
        [Required]
        public List<int> OrderedProductsIds { get; set; }

        [Required]
        public int AddressId { get; set; }
    }
}
