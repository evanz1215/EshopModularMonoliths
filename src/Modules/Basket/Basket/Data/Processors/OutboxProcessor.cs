using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Basket.Data.Processors;

public class OutboxProcessor(IServiceProvider serviceProvider, IBus bus, ILogger<OutboxProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();

                // 找出未處理的Outbox訊息
                var outboxMessages = await dbContext.OutboxMessages
                    .Where(x => x.ProcessedOn == null)
                    .ToListAsync(stoppingToken);

                foreach (var message in outboxMessages)
                {
                    var eventType = Type.GetType(message.Type);
                    if (eventType is null)
                    {
                        logger.LogWarning("Could not resolve type: {Type}", message.Type);
                        continue;
                    }

                    var eventMessage = JsonSerializer.Deserialize(message.Content, eventType);

                    if (eventMessage is null)
                    {
                        logger.LogWarning("Could not deserialize message: {Message}", message.Content);
                        continue;
                    }

                    await bus.Publish(eventMessage, stoppingToken);

                    message.ProcessedOn = DateTime.UtcNow;

                    logger.LogInformation("Successfully processed outbox message with ID: {Id}", message.Id);
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing outbox messages");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}