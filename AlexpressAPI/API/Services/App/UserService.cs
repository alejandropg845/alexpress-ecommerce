using API.DTOs.UsersDto;
using API.Entities;
using API.Interfaces.Services;
using API.Interfaces.Services.User;
using API.Responses.User;
using API.UnitsOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace API.Services.App
{
    public class UserService : IUserWriteService, IUserReadService
    {
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        
        public UserService( 
            UserManager<AppUser> userManager, 
            ITokenService tokenService, 
            SignInManager<AppUser> sign,
            IAuthUnitOfWork authUoW
        )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = sign;
            _authUnitOfWork = authUoW;
        }

        public async Task<RegisterUserResponse> RegisterAsync(RegisterUserDto dto)
        {
            var response = new RegisterUserResponse();

            var emailExists = await _authUnitOfWork.UserRepository.UserExistsByEmailAsync(dto.Email);

            response.EmailExists = emailExists;

            if (emailExists) return response;

            var usernameExists = await _authUnitOfWork.UserRepository.UserExistsByNameAsync(dto.Username);

            response.UserExists = usernameExists;

            if (usernameExists) return response;

            var appUser = new AppUser
            {
                UserName = dto.Username,
                Email = dto.Email
            };

            using var transaction = await _authUnitOfWork.BeginTransactionAsync();
         
            try
            {
                // 1
                var result = await _userManager.CreateAsync(appUser, dto.Password);

                if (!result.Succeeded)
                {
                    response.ErrorMessage = result.Errors.FirstOrDefault()?.Description;
                    return response;
                }

                // 2
                await SendConfirmationEmailAsync(appUser);

                // 3
                var result2 = await _userManager.AddToRoleAsync(appUser, "user");

                if (!result2.Succeeded)
                {
                    response.ErrorMessage = result.Errors.FirstOrDefault()?.Description;
                    await transaction.RollbackAsync();
                    return response;
                }

                await transaction.CommitAsync();

            } catch 
            {
                await transaction.RollbackAsync();
                throw;
            }

            return response;
        }

        private async Task SendConfirmationEmailAsync(AppUser appUser)
        {
            string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            var outboxMessage = Extensions.CreateTokenOutboxMessage(
                confirmationToken,
                appUser.Email!,
                "emailConfirmation"
            );

            _authUnitOfWork.OutboxMessageRepository.SaveOutboxMessage(outboxMessage);
            await _authUnitOfWork.SaveContextsChangesAsync();
        }

        public async Task<LoginResponse> LoginAsync(LoginDto dto)
        {
            var response = new LoginResponse();

            var appUser = await _userManager.FindByNameAsync(dto.Username);

            response.IsCorrect = appUser is not null;

            if (appUser is null) return response;

            var result = await _signInManager.CheckPasswordSignInAsync(appUser, dto.Password, false);

            response.IsCorrect = result.Succeeded;

            if (!result.Succeeded) return response;

            bool isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(appUser);
            response.IsEmailConfirmed = isEmailConfirmed;

            if (!isEmailConfirmed)
            {
                await SendConfirmationEmailAsync(appUser);
                return response;
            }

            bool twoFactorEnabled = await IsTwoFactorEnabledAsync(appUser.Id);

            response.IsTwoFactorEnabled = twoFactorEnabled;

            if (twoFactorEnabled)
            {
                response.PartialToken = _tokenService.CreatePartialToken(appUser.Id);
                return response;
            }

            var role = await _userManager.GetRolesAsync(appUser);

            RToken newRToken = Extensions.CreateRefreshToken(appUser.Id);

            RToken refreshToken = await _authUnitOfWork.RTokenRepository.CreateRefreshToken(newRToken);
            
            string accessToken = _tokenService.CreateToken(
                appUser.Id, 
                appUser.Email!, 
                appUser.UserName!, 
                role.First()
            );

            response.AccessToken = accessToken;
            response.RefreshToken = refreshToken.RefreshToken;
            return response;
        }

        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null) return false;

            await _userManager.ConfirmEmailAsync(user, token);

            return true;
        }
        public async Task RequestPasswordResetAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                await Task.Delay(new Random().Next(100, 300));
                return;
            };


            var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var outboxMessage = Extensions.CreateTokenOutboxMessage(
                passwordToken,
                email,
                "changePassword"
            );

            _authUnitOfWork.OutboxMessageRepository.SaveOutboxMessage(outboxMessage);

            await _authUnitOfWork.SaveContextsChangesAsync();

        }

        public async Task<ChangePasswordFromEmailResponse> ChangePasswordFromEmailAsync(ChangePasswordDto dto)
        {
            var response = new ChangePasswordFromEmailResponse();

            bool passwordMatch = dto.Password1 == dto.Password2;

            response.PasswordsMatch = passwordMatch;
            if (!passwordMatch) return response;

            var user = await _userManager.FindByEmailAsync(dto.Email);

            response.IsValidSpecifiedUser = user is not null;
            if (!response.IsValidSpecifiedUser) return response;

            var result = await _userManager.ResetPasswordAsync(user!, dto.Token, dto.Password1);

            if (!result.Succeeded)
            {
                response.ErrorMessage = result.Errors.FirstOrDefault()?.Description;
                return response;
            }

            return response;

        }

        public async Task<ChangePasswordInsideAppResponse> ChangePasswordInsideAppAsync(string userId, string currentPass, string pass1, string pass2)
        {
            var response = new ChangePasswordInsideAppResponse();

            bool passwordsMatch = pass1 == pass2;

            response.PasswordsMatch = passwordsMatch;
            if (!passwordsMatch) return response;

            
            var user = await _userManager.FindByIdAsync(userId);

            response.UserExists = user is not null;
            if (user is null) return response;

            var result = await _userManager.ChangePasswordAsync(user, currentPass, pass1);

            if (!result.Succeeded)
            {
                response.ErrorMessage = result.Errors.FirstOrDefault()?.Description;
                return response;
            }

            return response;
        }

        public async Task<Get2FAKeyResponse> Get2FAKey(string userId)
        {
            var response = new Get2FAKeyResponse();

            var user = await _userManager.FindByIdAsync(userId);

            response.UserExists = user is not null;
            if (user is null) return response;

            string? key = await _userManager.GetAuthenticatorKeyAsync(user);

            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            response.Key = $"otpauth://totp/Alexpress:{user.Email}?secret={key}&issuer=Alexpress";

            return response;
        }

        public async Task<bool> IsTwoFactorEnabledAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null) return false;

            return await _userManager.GetTwoFactorEnabledAsync(user);
        }

        public async Task<Enable2FAResponse> Enable2FA(string userId, string twoFAtoken)
        {
            var response = new Enable2FAResponse();

            var user = await _userManager.FindByIdAsync(userId);

            response.UserExists = user is not null;
            if (user is null) return response;

            response.IsTwoFactorAlreadyEnabled = await IsTwoFactorEnabledAsync(user.Id);

            if (response.IsTwoFactorAlreadyEnabled) return response;

            string provider = _userManager.Options.Tokens.AuthenticatorTokenProvider;

            bool isTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, provider, twoFAtoken);

            response.IsTokenValid = isTokenValid;
            if (!isTokenValid) return response;

            
            await _userManager.SetTwoFactorEnabledAsync(user, true);
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 5);

            response.RecoveryCodes = recoveryCodes;

            return response;
        }

        public async Task<LoginTwoFactorResponse> LoginWithTwoFactorAuthAsync(string userId, string code)
        {
            var response = new LoginTwoFactorResponse();

            var user = await _userManager.FindByIdAsync(userId);

            response.UserExists = user is not null;
            if (user is null) return response;

            string provider = _userManager.Options.Tokens.AuthenticatorTokenProvider;

            bool isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, 
                provider,
                code
            );

            response.CorrectCode = isValid;
            if (!isValid) return response;

            var t = Extensions.CreateRefreshToken(user.Id);

            RToken rToken = await _authUnitOfWork.RTokenRepository.CreateRefreshToken(t);

            response.AccessToken = _tokenService.CreateToken(user.Id, user.Email!, user.UserName!, "user");
            response.RefreshToken = rToken.RefreshToken;

            return response;
        }
    }
}
