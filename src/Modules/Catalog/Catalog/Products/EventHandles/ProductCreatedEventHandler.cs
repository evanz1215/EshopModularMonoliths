namespace Catalog.Products.EventHandles;

public class ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger) : INotificationHandler<ProductCreatedEvent>
{
    public Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        // TODO: publish product price changed integration event for update basket prices
        logger.LogInformation($"Domain Event handled: {notification.GetType().Name}");

        return Task.CompletedTask;
    }
}