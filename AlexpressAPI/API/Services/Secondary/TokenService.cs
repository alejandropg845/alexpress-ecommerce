using API.Entities;
using API.Interfaces.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services.Secondary
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));
        }
        public string CreateToken(string userId, string email, string username, string role)
        {
            var signing = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var claimsIdentity = new List<Claim>
            {
              new(ClaimTypes.Email, email),
              new(ClaimTypes.GivenName, username),
              new(ClaimTypes.NameIdentifier, userId),
              new(ClaimTypes.Role, role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claimsIdentity),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = signing,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"],
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string CreatePartialToken(string userId)
        {
            var signing = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var claimsIdentity = new List<Claim> { new Claim("pre_auth_id", userId) };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claimsIdentity),
                Expires = DateTime.UtcNow.AddMinutes(2),
                SigningCredentials = signing,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"],
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}