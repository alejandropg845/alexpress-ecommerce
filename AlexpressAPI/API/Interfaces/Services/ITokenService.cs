using API.Entities;

namespace API.Interfaces.Services
{
    public interface ITokenService
    {
        string CreateToken(string userId, string email, string username, string role);
        string CreatePartialToken(string userId);
    }
}
