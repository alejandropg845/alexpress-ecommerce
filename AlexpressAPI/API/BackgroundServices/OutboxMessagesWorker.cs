using API.Interfaces.Services;
using System.Diagnostics;

namespace API.BackgroundServices
{
    public class OutboxMessagesWorker : BackgroundService
    {
        private readonly ILogger<OutboxMessagesWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public OutboxMessagesWorker(IServiceScopeFactory serviceScopeFactory, ILogger<OutboxMessagesWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();

                    var outboxMessageService = scope.ServiceProvider.GetRequiredService<IOutboxMessageService>();

                    await outboxMessageService.ProcessOutboxMessagesAsync();
                    _logger.LogInformation("OutboxMessage processed successfully");
                    await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
                } catch (OperationCanceledException e)
                {
                    ShowException(e.Message, e.StackTrace);
                }
                catch (Exception ex)
                {
                    ShowException(ex.Message, ex.StackTrace);
                }

            }
        }

        private void ShowException(string exceptionMsg, string? stackStrace)
        {
            _logger.LogError("Error while trying to process outboxMessage in BackgroundService.\nException: {Msg}\nStackTrace: {StackTrace}",
                exceptionMsg, stackStrace ?? "No StackTrace"
            );
        }
    }
}
