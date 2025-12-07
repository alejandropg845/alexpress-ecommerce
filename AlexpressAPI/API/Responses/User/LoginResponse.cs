namespace API.Responses.User
{
    public class LoginResponse
    {
        public bool IsCorrect { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public string PartialToken { get; set; }
    }
}
