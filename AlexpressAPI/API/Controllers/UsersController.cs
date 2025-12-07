using API.DTOs.UsersDto;
using API.Interfaces.Services;
using API.Interfaces.Services.User;
using API.Payloads.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserWriteService _userService;
        private readonly IUserReadService _userReadService;
        public UsersController(IUserWriteService userWriteService, IUserReadService userReadService)
        {
            _userService = userWriteService;
            _userReadService = userReadService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] RegisterUserDto dto)
        {
            var response = await _userService.RegisterAsync(dto);

            if (response.UserExists) return BadRequest(new { Message = "This user is not available" });

            if (response.EmailExists) return BadRequest(new { Message = "This email is already in use" });

            if (response.ErrorMessage is not null) return BadRequest(new { Message = response.ErrorMessage });

            return Ok(new { Message = "Registration succeedeed. Check your email to verify your account" });
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginUser([FromBody] LoginDto dto)
        {
            var response = await _userReadService.LoginAsync(dto);

            if (!response.IsCorrect) return BadRequest(new { Message = "Incorrect credentials" });

            if (!response.IsEmailConfirmed) return BadRequest(new { Message = "Your email is not confirmed. Check your email." });

            if (response.IsTwoFactorEnabled) return Ok(new { Ok = false, response.PartialToken, response.IsTwoFactorEnabled });

            return Ok(new
            {
                Ok = true,
                response.AccessToken,
                response.RefreshToken
            });
        }

        [HttpPost("loginTwoFactor/{code}")]
        [Authorize]
        public async Task<ActionResult> LoginTwoFactorAuthentication([FromRoute] string code)
        {
            string? userId = User.FindFirstValue("pre_auth_id");

            if (userId is null) return Unauthorized();

            var response = await _userReadService.LoginWithTwoFactorAuthAsync(userId, code);

            if (!response.UserExists) return BadRequest();

            if (!response.CorrectCode) return BadRequest(new { Message = "Invalid code" });

            return Ok(new
            {
                Ok = true,
                response.AccessToken,
                response.RefreshToken
            });
        }

        [HttpPost("confirmEmail")]
        public async Task<ActionResult> ConfirmEmail([FromBody] EmailToken ec)
        {
            bool allOk = await _userService.ConfirmEmailAsync(ec.Email, ec.Token);

            if (!allOk) return BadRequest(new { Message = "This user doesn't exist" });

            return NoContent();
        }

        [HttpPost("requestChangePassword")]
        public async Task<ActionResult> RequestChangePassword([FromBody] RequestPasswordResetDto dto)
        {
            await _userReadService.RequestPasswordResetAsync(dto.Email);

            return NoContent();
        }

        [HttpPost("changePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var response = await _userService.ChangePasswordFromEmailAsync(dto);

            if (!response.PasswordsMatch) return BadRequest(new { Message = "Passwords don't match" });

            if (!response.IsValidSpecifiedUser) return Forbid();

            if (response.ErrorMessage is not null) 
                
                return BadRequest(new { Message = response.ErrorMessage });

            return NoContent();
        }

        [HttpPut("changePasswordInApp")]
        [Authorize]
        public async Task<ActionResult> ChangePasswordInApp([FromBody] ChangePasswordInAppDto dto)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null) return Unauthorized();

            var response = await _userService.ChangePasswordInsideAppAsync(
                userId,
                dto.CurrentPassword,
                dto.Pass1,
                dto.Pass2
            );

            if (!response.UserExists) return Unauthorized();

            if (!response.PasswordsMatch) return BadRequest(new { Message = "Passwords don't match" });

            if (response.ErrorMessage is not null)
                return BadRequest(new { Message = response.ErrorMessage });

            return Ok(new { Message = "Done" });
        }

        [HttpGet("get2FAKey")]
        [Authorize]
        public async Task<ActionResult> Get2FAKey()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null) return Unauthorized();

            var response = await _userReadService.Get2FAKey(userId);

            if (!response.UserExists) return Unauthorized();

            return Ok(new { response.Key });
        }

        [HttpPost("set2FA/{faCode}")]
        [Authorize]
        public async Task<ActionResult> Set2FAauthentication([FromRoute] string faCode)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null) return Unauthorized();

            var response = await _userService.Enable2FA(userId, faCode);

            if (!response.UserExists) return Unauthorized();

            if (!response.IsTokenValid) return BadRequest(new { Message = "Token is not valid" });
            
            if (response.IsTwoFactorAlreadyEnabled) return BadRequest(new { Message = "2FA is already enabled" });

            return Ok(new { response.RecoveryCodes });
        }

        [HttpGet("credentials")]
        [Authorize]
        public ActionResult GetUsername()
        {
            string? username = User.FindFirstValue(ClaimTypes.GivenName);

            return Ok(new { username });
        }

        [HttpGet("isTwoFactorEnabled")]
        [Authorize]
        public async Task<ActionResult> GetTwoFactorEnabled()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null) return Unauthorized();

            bool isFactor = await _userReadService.IsTwoFactorEnabledAsync(userId);

            return Ok(new { IsTwoFactorEnabled = isFactor });
        }
    }
}
