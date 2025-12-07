namespace API.DTOs.UsersDto
{
    public class LoginTwoFactorResponse
    {
        public bool UserExists { get; set; }
        public bool CorrectCode { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
