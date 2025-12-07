using API.Entities;

namespace API.DTOs.ReviewDTO
{
    public class ReviewItemDto
    {
        public DateTimeOffset CreatedAt { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string Author { get; set; }
    }
}
