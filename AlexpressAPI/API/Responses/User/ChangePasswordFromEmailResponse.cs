namespace API.Responses.User
{
    public class ChangePasswordFromEmailResponse
    {
        public bool IsValidSpecifiedUser { get; set; }
        public bool PasswordsMatch { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
