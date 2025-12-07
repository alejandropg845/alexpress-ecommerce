using API.Entities;
using API.Interfaces.Repositories;
using API.Interfaces.Services;
using API.Payloads.Auth;
using API.Payloads.Order;
using System.Text.Json;

namespace API.Services.App
{
    public class OutboxMessageService : IOutboxMessageService
    {
        private readonly IOutboxMessageRepository _repo;
        private readonly IMailService _mailService;
        private readonly ILogger<OutboxMessageService> _logger;
        public OutboxMessageService(IOutboxMessageRepository r, IMailService mailService, ILogger<OutboxMessageService> logger)
        {
            _repo = r;
            _mailService = mailService;
            _logger = logger;
        }

        public async Task ProcessOutboxMessagesAsync()
        {
            var outboxMessages = await _repo.GetOutboxMessagesAsync();

            foreach(var outboxMessage in outboxMessages)
            {
                switch (outboxMessage.Type)
                {
                    case "orderMail":

                        var orderMailPayload = JsonSerializer.Deserialize<OrderMail>(outboxMessage.Payload)!;

                        await _mailService.SendSummaryAsync(orderMailPayload);

                    break;

                    case "emailConfirmation":

                        var emailConfirmationPayload = JsonSerializer.Deserialize<EmailToken>(outboxMessage.Payload)!;

                        await _mailService.SendEmailTokenAsync(
                            emailConfirmationPayload,
                            "email-confirmation",
                            "confirm your email.",
                            "Confirm your email"
                        );

                        await _repo.DeleteOutboxMessageAsync(outboxMessage);
                        
                    continue;

                    case "changePassword":

                        var changePasswordPayload = JsonSerializer.Deserialize<EmailToken>(outboxMessage.Payload)!;

                        await _mailService.SendEmailTokenAsync(
                            changePasswordPayload,
                            "change-password",
                            "change your password.",
                            "Change your password"
                        );

                        await _repo.DeleteOutboxMessageAsync(outboxMessage);

                    continue;

                }

                outboxMessage.IsProcessed = true;
                outboxMessage.ProcessedTime = DateTime.UtcNow;
                await _repo.SaveContextChangesAsync();

            }

        }
    }
}
