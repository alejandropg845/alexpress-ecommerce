namespace API.Responses.User
{
    public record ChangePasswordInsideAppResponse
    {
        public bool UserExists { get; set; }
        public bool PasswordsMatch { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
