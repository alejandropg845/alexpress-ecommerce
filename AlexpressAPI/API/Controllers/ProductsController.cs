using API.DTOs.ProductDto;
using API.Entities;
using API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductsController(IProductService repo)
        {
            _service = repo;
        }

        //[HttpGet("keepAlive")]
        //public async Task<ActionResult> KeepAlive()
        //{
        //    return Ok(await _service.KeepAliveAsync());
        //}

        [HttpGet("userProducts")]
        [Authorize]
        public async Task<ActionResult> GetUserProducts()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized();

            var products = await _service.GetUserProductsAsync(userId);

            return Ok(products);

        }

        [HttpGet("getProducts/{title}")]
        [AllowAnonymous]
        public async Task<ActionResult<ToProductDto>> GetProducts([FromRoute] string? title, [FromQuery] int categoryId, [FromQuery] decimal price)
        {
            string? userId = User.FindFirstValue(ClaimTypes.GivenName);

            var products = await _service.GetProductsAsync(title, categoryId, userId, price);

            return Ok(products);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ToProductDto>> GetProductById([FromRoute] int id)
        {
            var product = await _service.GetProductAsync(id);

            return product is null ?
                NotFound(new { Message = "This product no longer exists" })
            : Ok(product);
        }

        [HttpGet("productToUpdate/{id}")]
        [Authorize]
        public async Task<ActionResult> GetProductDtoToUpdate([FromRoute] int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null) return Unauthorized();

            return Ok(await _service.GetProductDtoToUpdateAsync(id, userId));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            string? username = User.FindFirstValue(ClaimTypes.GivenName);
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (username is null || userId is null) return Unauthorized();

            var r = await _service.CreateProductAsync(dto, userId, username);

            if (!r.UserExists) return Unauthorized(new { Message = "You have been banned for uploading explicit content" });

            if (r.IsExplicitDescription)
                return BadRequest(new { Message = "Your description contains inappropriated content" });

            if (r.IsExplicitImage) return BadRequest(new { Message = "Please avoid uploading NFSW images or inappropiated content" });

            if (r.IsExplicitTitle) return BadRequest(new { Message = "Your title contains inappropriated content" });

            return CreatedAtAction(nameof(GetProductById), new { id = r.ProductDto.Id }, r.ProductDto);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult> UpdateProductById([FromRoute] int id, [FromBody] UpdateProductDto dto)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null) return Unauthorized();

            var r = await _service.UpdateProductAsync(id, dto, userId);

            if (!r.UserExists) 
                return Unauthorized
                    (new { Message = "You have been banned for uploading explicit content" });

            if (r.IsExplicitContent)
                return BadRequest(new { Message = "Your product info. contains inappropriated content" });

            if (!r.ProductExists) return NotFound(new { Message = "The product you want to update doesn't exist" });


            return CreatedAtAction(nameof(GetProductById), new { id = r.ProductDto.Id }, r.ProductDto);

        }

        [HttpDelete("{productId:int}")]
        [Authorize]
        public async Task<ActionResult> DeleteProductById([FromRoute] int productId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null) return Unauthorized();

            var r = await _service.DeleteProductAsync(productId, userId);

            if(r.IsUserDisabled) return Unauthorized
                    (new { Message = "You have been banned for uploading explicit content" });

            if (!r.ProductExists)
            
                return NotFound(new {Message = "This product no longer exists"});
            

            return NoContent();
        }

        [HttpDelete("deleteFromCloudinary/{publicId}")]
        [Authorize]
        public async Task<ActionResult> DeleteFromCloudinary([FromRoute] string publicId)
        {
            await _service.DeleteFromCloudinaryAsync(publicId);

            return NoContent();
        }
    }
}
