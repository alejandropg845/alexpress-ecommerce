using API.DTOs.OrderDTO;
using API.Responses.Order;
using Microsoft.Extensions.Primitives;

namespace API.Interfaces.Services
{
    public interface IOrderService
    {
        Task<IReadOnlyList<OrderDto>> GetOrdersAsync(string userId);
        Task HandleStripeWebHook(string json, StringValues stripeSignature);
        Task<CreateOrderResponse> CreateOrderAsync(int addressId, string userId, string email, string stripeSessionId, string username);
        Task<SummarizeOrderResponse> SummarizeOrderAsync(string username, string userId, int addressId, string email);
        Task<ReviewOrderResponse> ReviewOrderAsync(CreateReviewDto dto, string username, string userId);
    }
}
