using API.Entities;
using API.Interfaces.Repositories;
using API.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace API.Services.App
{
    public class RTokenService : IRTokenService
    {
        private readonly IRTokenRepository _repo;
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        public RTokenService(IRTokenRepository repo, ITokenService tokenService, UserManager<AppUser> u)
        {
            _repo = repo;
            _tokenService = tokenService;
            _userManager = u;
        }

        public async Task<(string? RToken, string? AToken)> GetAccessTokenAsync(string refreshToken)
        {
            var rToken = await _repo.GetRefreshTokenAsync(refreshToken);

            if (rToken == null) return (null, null);
            
            if (rToken.IsRevoked) return (null, null);

            bool isExpired = rToken.ExpirationTime < DateTimeOffset.UtcNow;

            if (isExpired)
            {
                await _repo.RevokeRTokenAsync(rToken);
                return new(null, null);
            }

            var user = await _userManager.FindByIdAsync(rToken.AppUserId);

            if (user == null) return (null, null);

            var role = await _userManager.GetRolesAsync(user);

            string accessToken = _tokenService.CreateToken(user.Id, user.Email!, user.UserName!, role.First());

            return new(rToken.RefreshToken, accessToken);
        }
    }
}
