using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Products.EventHandles;

internal class ProductPriceChangedHandler(ILogger<ProductPriceChangedHandler> logger) : INotificationHandler<ProductPriceChangedEvent>
{
    public Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Domain Event handled: {notification.GetType().Name}");
        return Task.CompletedTask;
    }
}