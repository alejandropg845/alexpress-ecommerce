using System.ComponentModel.DataAnnotations;

namespace API.DTOs.RTokenDto
{
    public record GetRTokenDto
    {
        [Required] public string RefreshToken { get; set; }
    }
}
