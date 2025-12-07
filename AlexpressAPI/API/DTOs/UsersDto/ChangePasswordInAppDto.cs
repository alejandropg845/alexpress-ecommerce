using System.ComponentModel.DataAnnotations;

namespace API.DTOs.UsersDto
{
    public record ChangePasswordInAppDto
    {
        [Required] public string CurrentPassword { get; set; }
        [Required] public string Pass1 { get; set; }
        [Required] public string Pass2 { get; set; }
    }
}
