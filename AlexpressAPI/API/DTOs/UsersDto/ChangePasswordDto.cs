using System.ComponentModel.DataAnnotations;

namespace API.DTOs.UsersDto
{
    public record ChangePasswordDto
    {
        [Required] public string Email { get; set; }
        
        
        [Required]
        [MinLength(5)]
        public string Password1 { get; set; }

        [Required]
        [MinLength(5)]
        public string Password2 { get; set; }

        [Required] public string Token { get; set; }
    }
}
