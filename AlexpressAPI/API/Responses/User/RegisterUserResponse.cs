namespace API.Responses.User
{
    public class RegisterUserResponse
    {
        public bool EmailExists { get; set; }
        public bool UserExists { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
