using API.Entities;

namespace API.Interfaces.Services
{
    public interface IRTokenService
    {
        Task<(string? RToken, string? AToken)> GetAccessTokenAsync(string refreshToken);
    }
}
