using System.ComponentModel.DataAnnotations;

namespace API.DTOs.OrderDTO
{
    public record CreateReviewDto
    {
        [Required]
        [Range(1, 5)]
        public int Rating {  get; set; }

        [Required]
        [MaxLength(150)]
        public string Comment { get; set; }

        [Required]
        public int OrderId { get; set; }

    }
}
