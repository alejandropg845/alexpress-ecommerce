using API.DTOs.CouponDTO;
using API.DTOs.ReviewDTO;

namespace API.DTOs.AdminDTO
{
    public class ProductsAdminDto
    {
        public int Id { get; set; }
        public List<string>? Images { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Username { get; set; }
    }
}
