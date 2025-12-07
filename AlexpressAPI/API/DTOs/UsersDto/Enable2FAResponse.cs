namespace API.DTOs.UsersDto
{
    public class Enable2FAResponse
    {
        public bool UserExists { get; set; }
        public bool IsTokenValid { get; set; }
        public bool IsTwoFactorAlreadyEnabled { get; set; }
        public IEnumerable<string>? RecoveryCodes { get; set; }
    }
}
