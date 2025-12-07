namespace API.DTOs.UsersDto
{
    public class NewUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool Ok { get; set; }
    }
}
