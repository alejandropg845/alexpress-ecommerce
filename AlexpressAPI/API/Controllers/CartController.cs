using API.DbContexts;
using API.DTOs.CartDTO;
using API.DTOs.CartProductsDTO;
using API.DTOs.ProductDto;
using API.Entities;
using API.Interfaces.Repositories;
using API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<CartProductDto>> GetUserCart()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost()]
        [Authorize]
        public async Task<ActionResult> AddProductToCart([FromBody] AddToCartDto dto)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var response = await _cartService.AddToCartAsync(userId, dto);

            if (!response.ProductExists)
                
                return NotFound(new { Message = "This product no longer exists" });

            if (response.OwnProduct)

                return BadRequest(new { Message = "You cannot add your products into the cart" });

            if (response.NoMoreStock)

                return BadRequest(new { Message = "This product doesn't have more stock" });

            if (response.TwoOrMoreUnitsRequired)

                return BadRequest(new { Message = "This product requires at least 2 units" });

            return Ok(new
            {
                response.CartProduct,
                response.IsProductRemoved,
                response.Summary,
                response.CartProduct.CartId
            });
            
        }

        [HttpDelete("{productId:int}")]
        [Authorize]
        public async Task<ActionResult> RemoveFromCart([FromRoute] int productId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null) return Unauthorized();

            var r = await _cartService.RemoveCartProductAsync(productId, userId);

            return Ok(new
            {
                r.Summary
            });
        }
    }
}
