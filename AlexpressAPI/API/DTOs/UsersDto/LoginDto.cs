using System.ComponentModel.DataAnnotations;

namespace API.DTOs.UsersDto
{
    public class LoginDto
    {
        [Required] [MaxLength(20)] public string Username { get; set; }
        [Required] [MaxLength(100)] public string Password { get; set; }
    }
}
