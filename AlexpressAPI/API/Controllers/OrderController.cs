using API.DTOs.OrderDTO;
using API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;
        public OrderController(IOrderService orderService)
        {
            _service = orderService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetOrders()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var orders = await _service.GetOrdersAsync(userId);

            return Ok(orders);
        }

        [HttpPost("createStripeOrder")]
        public async Task<ActionResult> CreateOrderWithStripe()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            Console.WriteLine("valor del json"+ json);

            try
            {
                var stripeSignature = Request.Headers["Stripe-Signature"];

                await _service.HandleStripeWebHook(json, stripeSignature);

                return NoContent();

            } 
            catch (StripeException) { return StatusCode(400); }
            catch (Exception) { throw; } 

        }

        [HttpPost("summarizeOrder/{addressId}")]
        [Authorize]
        public async Task<ActionResult> SummarizeOrder([FromRoute] int addressId)
        {
            string? username = User.FindFirstValue(ClaimTypes.GivenName);

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            string? email = User.FindFirstValue(ClaimTypes.Email);

            if (username == null || userId == null || email == null) return Unauthorized();

            var response = await _service.SummarizeOrderAsync(username, userId, addressId, email);

            if (!response.IsCart) return BadRequest(new { Message = "Your cart is empty. Add products first" });
            if (!response.UserExists) return Unauthorized();


            return Ok(new { response.SessionUrl });
        }

        [HttpPost("reviewOrder")]
        [Authorize]
        public async Task<ActionResult> ReviewOrder([FromBody] CreateReviewDto dto)
        {
            string? username = User.FindFirstValue(ClaimTypes.GivenName);
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (username is null || userId is null) return Unauthorized();

            var r = await _service.ReviewOrderAsync(dto, username, userId);

            if (!r.OrderExists) return BadRequest(new { Message = "This order doesn't exist" });

            return NoContent();

        }
    }
}
