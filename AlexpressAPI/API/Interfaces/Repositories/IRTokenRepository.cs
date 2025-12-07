using API.Entities;

namespace API.Interfaces.Repositories
{
    public interface IRTokenRepository
    {
        Task<RToken?> GetRefreshTokenAsync(string refreshToken);
        Task<RToken> CreateRefreshToken(RToken r);
        Task RevokeRTokenAsync(RToken r);
    }
}
