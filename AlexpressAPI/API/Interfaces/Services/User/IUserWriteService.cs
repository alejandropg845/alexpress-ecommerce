using API.DTOs.UsersDto;
using API.Responses.User;

namespace API.Interfaces.Services
{
    public interface IUserWriteService
    {
        public Task<RegisterUserResponse> RegisterAsync(RegisterUserDto dto);
        Task<bool> ConfirmEmailAsync(string email, string token);
        Task<ChangePasswordFromEmailResponse> ChangePasswordFromEmailAsync(ChangePasswordDto dto);
        Task<ChangePasswordInsideAppResponse> ChangePasswordInsideAppAsync(string userId, string currentPass, string pass1, string pass2);
        Task<Enable2FAResponse> Enable2FA(string userId, string twoFAtoken);
    }
}
