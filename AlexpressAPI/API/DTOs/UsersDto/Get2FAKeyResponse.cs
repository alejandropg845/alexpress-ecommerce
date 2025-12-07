namespace API.DTOs.UsersDto
{
    public class Get2FAKeyResponse
    {
        public bool UserExists { get; set; }
        public string Key { get; set; }
    }
}
