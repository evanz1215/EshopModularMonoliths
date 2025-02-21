using MassTransit;
using Shared.Messaging.Events;

namespace Catalog.Products.EventHandles;

internal class ProductPriceChangedHandler(IBus bus, ILogger<ProductPriceChangedHandler> logger) : INotificationHandler<ProductPriceChangedEvent>
{
    public async Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Domain Event handled: {notification.GetType().Name}");

        // Publish product price changed integration event for update basket prices
        var integrationEvent = new ProductPriceChangedIntegrationEvent
        {
            ProductId = notification.Product.Id,
            Name = notification.Product.Name,
            Category = notification.Product.Category,
            Description = notification.Product.Description,
            ImageFile = notification.Product.ImageFile,
            Price = notification.Product.Price // set updated product price
        };

        await bus.Publish(integrationEvent, cancellationToken);
    }
}