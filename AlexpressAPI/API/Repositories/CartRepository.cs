using API.DbContexts;
using API.DTOs.CartDTO;
using API.DTOs.CartProductsDTO;
using API.DTOs.ProductDto;
using API.Entities;
using API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly CartDbContext _context;
        public CartRepository(CartDbContext context)
        {
            _context = context;
        }
        public async Task<ToCartDto?> GetCartAsync(string userId)
        {
            var cart = await _context.Cart
                .Where(c => c.AppUserId == userId)
                .Select(c => new ToCartDto
                {
                    Id = c.Id,
                    Summary = c.Summary,
                    CartProducts = c.CartProducts.Select(cp => new CartProductDto
                    {
                        CustomizedDiscount = cp.CustomizedDiscount,
                        Image = cp.Image,
                        CouponName = cp.CouponName,
                        ProductId = cp.ProductId,
                        Quantity = cp.Quantity,
                        ShippingPrice = cp.ShippingPrice,
                        Title = cp.Title,
                        NewPrice = cp.NewPrice
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return cart;
        }

        public async Task<Cart?> GetCartIfExistsAsync(string userId)
        {
            var cart = await _context.Cart
                .FirstOrDefaultAsync(c => c.AppUserId == userId);

            return cart;
        }
        public void AddCart(Cart cart) => _context.Cart.Add(cart);

        public async Task<CartProduct?> GetProductInCartIfExistsAsync(int cartId, int productId)
        => await _context.CartProducts.FirstOrDefaultAsync(cp => cp.CartId == cartId && cp.ProductId == productId);

        public async Task SaveContextChangesAsync() => await _context.SaveChangesAsync();

        public async Task<ToCartDto?> GetOrderedCartDtoAsync(string userId)
        {
            return await _context.Cart
                .Where(c => c.AppUserId == userId)
                .Select(c => new ToCartDto
                {
                    Id = c.Id,
                    AppUserId = c.AppUserId,
                    Summary = c.Summary,
                    CartProducts = c.CartProducts.Select(cp => new CartProductDto
                    {
                        ProductId = cp.ProductId,
                        Image = cp.Image,
                        Quantity = cp.Quantity,
                        Price = cp.NewPrice,
                        Title = cp.Title,
                        CouponName = cp.CouponName,
                        CustomizedDiscount = cp.CustomizedDiscount,
                        ShippingPrice = cp.ShippingPrice,
                    }).ToList()
                }).FirstOrDefaultAsync(c => c.AppUserId == userId);

        }

        public async Task EmptyUserCartAsync(int cartId)
        {
            var task1 = _context.CartProducts
                .Where(cp => cp.CartId == cartId)
                .ExecuteDeleteAsync();

            var task2 = _context.Cart
                .Where(cp => cp.Id == cartId)
                .ExecuteUpdateAsync(c => c.SetProperty(c => c.Summary, 0));

            await Task.WhenAll(task1, task2);
        }

        public async Task<bool> CartExistsAndContainsProductsAsync(string userId)
        {
            var (cartId, appUserId) = await _context.Cart
                .Select(c => new ValueTuple<int, string>(c.Id, c.AppUserId))
                .FirstOrDefaultAsync(tuple => tuple.Item2 == userId);

            if (cartId is 0) return false;

            bool cartProducts = await _context.CartProducts.AnyAsync(cp => cp.CartId == cartId);

            if (!cartProducts) return cartProducts;

            return true;
        }
    }
}
