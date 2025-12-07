using System.ComponentModel.DataAnnotations;

namespace API.DTOs.CartDTO
{
    public class ToProductInCartDto
    {
        [Required]
        public string Image { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]        
        public decimal Price {  get; set; }

        [Required]
        public decimal ShippingPrice { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }
}
