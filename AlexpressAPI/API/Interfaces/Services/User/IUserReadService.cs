using API.DTOs.UsersDto;
using API.Responses.User;

namespace API.Interfaces.Services.User
{
    public interface IUserReadService
    {
        public Task<LoginResponse> LoginAsync(LoginDto dto);
        Task RequestPasswordResetAsync(string email);
        Task<Get2FAKeyResponse> Get2FAKey(string userId);
        Task<bool> IsTwoFactorEnabledAsync(string userId);
        Task<LoginTwoFactorResponse> LoginWithTwoFactorAuthAsync(string userId, string code);

    }
}
