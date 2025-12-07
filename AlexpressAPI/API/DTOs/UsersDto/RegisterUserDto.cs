using System.ComponentModel.DataAnnotations;

namespace API.DTOs.UsersDto
{
    public class RegisterUserDto
    {
        [Required]
        [MaxLength(20)]
        public string Username { get; set; }
        
        [Required]
        [MaxLength(60)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Min. characters for password is 5")]
        [MaxLength(100, ErrorMessage = "Max. password length is 120")]
        public string Password { get; set; }
    }
}
