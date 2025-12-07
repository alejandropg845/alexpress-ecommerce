using API.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;
        public AdminController(IAdminService service)
        {
            _service = service;
        }
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetUsers()
        {
            return Ok(await _service.GetUsersAsync());
        }

        [HttpGet("products")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllProducts()
        {
            var products = await _service.GetAllProductsAsync();

            return Ok(products);
        }

        [HttpDelete("deleteProduct/{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProduct([FromRoute] int productId)
        {
            await _service.DeleteProductAsync(productId);
            return Ok();
        }

        [HttpPut("disableUser/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DisableUser(string userId)
        {
            await _service.DisableUserAsync(userId);
            return Ok();
        }

    }
}
