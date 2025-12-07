using API.DTOs.AddressesDTO;
using API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/address")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _service;
        public AddressController(IAddressService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetAllAddresses()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var addresses = await _service.GetAddressesAsync(userId);
            return Ok(addresses);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddAddress([FromBody] CreateAddressDto dto)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var addedAddress = await _service.AddAddressAsync(dto, userId);

            return Ok(addedAddress);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateAddress([FromRoute] int id, [FromBody] UpdateAddressDto dto)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var updatedAddress = await _service.UpdateAddressAsync(id, dto);

            if (updatedAddress is null) return NotFound(new { Message = "This address doesn't exist" });

            return Ok(updatedAddress);
        }

        
    }
}
