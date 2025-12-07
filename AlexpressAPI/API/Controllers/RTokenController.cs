using API.DTOs.RTokenDto;
using API.Interfaces.Services;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/rToken")]
    public class RTokenController : ControllerBase
    {
        private readonly IRTokenService _service;
        public RTokenController(IRTokenService s)
        {
            _service = s;
        }

        [HttpPost("getAccessToken")]
        public async Task<ActionResult> GetAccessToken([FromBody] GetRTokenDto dto)
        {
            var (rToken, accessToken) = await _service.GetAccessTokenAsync(dto.RefreshToken);

            if (rToken is null || accessToken is null) return Unauthorized();

            return Ok(new
            {
                RefreshToken = rToken,
                AccessToken = accessToken,
            });

        }
    }
}
