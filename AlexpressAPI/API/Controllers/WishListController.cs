
using API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/wishlist")]
    public class WishListController : ControllerBase
    {
        private readonly IWishlistService _service;
        public WishListController(IWishlistService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetWishlist()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var wishlist = await _service.GetWishlistAsync(userId);

            return Ok(wishlist);
        }

        [HttpPost("{productId:int}")]
        [Authorize]
        public async Task<ActionResult> AddToWishlistAsync([FromRoute] int productId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null) return Unauthorized();

            var response = await _service.AddToWishListAsync(userId, productId);

            if (response.IsUserProduct) return BadRequest(new { Message = "You cannot add your own product" });

            if (!response.ProductExists) return NotFound(new { Message = "This product doesn't exist" });

            if (response.IsProductInWishlist) return BadRequest(new { Message = "This product is already in your wishlist" });

            return Ok(response.AddedWishlistItem);
        }

        [HttpDelete("deleteFromWishlist/{productId}")]
        [Authorize]
        public async Task<ActionResult> RemoveFromWishlist([FromRoute] int productId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null) return Unauthorized();

            await _service.DeleteFromWishlistAsync(userId, productId);

            return Ok();
        }
    }
}
