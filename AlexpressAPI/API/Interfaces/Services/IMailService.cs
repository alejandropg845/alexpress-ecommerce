using API.DTOs.OrderDTO;
using API.Payloads.Auth;
using API.Payloads.Order;

namespace API.Interfaces.Services
{
    public interface IMailService
    {
        Task SendSummaryAsync(OrderMail payload);
        Task SendEmailTokenAsync(EmailToken payload, string angularPageName, string actionName, string subject);
    }
}
