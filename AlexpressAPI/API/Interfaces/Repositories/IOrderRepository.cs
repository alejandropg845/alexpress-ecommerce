using API.DTOs.OrderDTO;
using API.Entities;
using API.Payloads.Order;
using API.Responses.Order;
using Microsoft.EntityFrameworkCore;

namespace API.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<IReadOnlyList<OrderDto>> GetOrdersAsync(string userId);
        void SaveOrder(Order order);
        Task SaveOrderChangesAsync();
        Task<bool> IsOrderAlreadyCreatedAsync(string stripeSessionId);
        Task<OrderDto?> GetOrderDtoAsync(int orderId);
        Task SetOrderRatingAsync(int rating, int orderId);
        Task<OrderReviewDto?> GetOrderInfoForReviewAsync(int orderId, string userId);
    }
}
