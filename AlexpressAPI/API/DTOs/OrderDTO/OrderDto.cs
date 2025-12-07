using API.DTOs.AddressesDTO;

namespace API.DTOs.OrderDTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public AddressDto Address { get; set; }
        public int AddressId { get; set; }
        public int Rating {  get; set; }
        public decimal Summary { get; set; }
        public List<OrderedProductDto> OrderedProducts { get; set; }
    }
}
