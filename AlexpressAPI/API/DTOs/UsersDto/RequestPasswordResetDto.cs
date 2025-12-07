namespace API.DTOs.UsersDto
{
    public record RequestPasswordResetDto
    {
        public string Email { get; set; }
    }
}
