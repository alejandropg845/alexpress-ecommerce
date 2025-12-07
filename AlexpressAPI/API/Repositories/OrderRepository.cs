using API.DbContexts;
using API.DTOs.AddressesDTO;
using API.DTOs.OrderDTO;
using API.Entities;
using API.Interfaces.Repositories;
using API.Payloads.Order;
using API.Responses.Order;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;
        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<OrderDto>> GetOrdersAsync(string userId)
        {
            var orders = await _context.Orders
            .Where(o => o.AppUserId == userId)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                CreatedOn = o.CreatedOn,
                Summary = o.Summary,
                Rating = o.Rating,
                Address = new AddressDto
                {
                    City = o.Address.City,
                    Country = o.Address.Country,
                    FullName = o.Address.FullName,
                    Phone = o.Address.Phone,
                    PostalCode = o.Address.PostalCode,
                    Residence = o.Address.Residence
                },
                OrderedProducts = o.OrderedProducts.Select(o => new OrderedProductDto
                {
                    Image = o.Image,
                    Price = o.Price,
                    ProductId = o.ProductId,
                    Quantity = o.Quantity
                }).ToList()

            }).ToListAsync();

            return orders;
        }

        public void SaveOrder(Order order) => _context.Orders.Add(order);
        public async Task SaveOrderChangesAsync() => await _context.SaveChangesAsync();
        public async Task<OrderDto?> GetOrderDtoAsync(int orderId)
        {
            return await _context.Orders.Select(o => new OrderDto
            {
                Summary = o.Summary,
                OrderedProducts = o.OrderedProducts.Select(op => new OrderedProductDto
                {
                    Image = op.Image,
                    Price = op.Price,
                    Quantity = op.Quantity,
                    Title = op.Title
                }).ToList()
            })
            .FirstOrDefaultAsync(o => o.Id == orderId);
        }
        public async Task<bool> IsOrderAlreadyCreatedAsync(string stripeSessionId)
        => await _context.Orders.AnyAsync(o => o.StripeSessionId == stripeSessionId);

        public async Task SetOrderRatingAsync(int rating, int orderId)
        {
            await _context.Orders.ExecuteUpdateAsync(x => x.SetProperty(o => o.Rating, rating));
        }

        public async Task<OrderReviewDto?> GetOrderInfoForReviewAsync(int orderId, string userId)
        {
            var order = await _context.Orders
                .Where(o => o.Id == orderId && o.AppUserId == userId)
                .Select(o => new OrderReviewDto
                {
                    Id = o.Id,
                    ProductsIds = o.OrderedProducts.Select(o => o.ProductId).ToList(),
                })
                .FirstOrDefaultAsync();

            return order;
        }
    }
}
