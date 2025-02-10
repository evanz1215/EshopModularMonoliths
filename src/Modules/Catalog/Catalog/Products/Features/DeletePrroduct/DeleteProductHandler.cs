namespace Catalog.Products.Features.DeletePrroduct;

public record DeleteProductCommand(Guid ProductId) : ICommand<DeleteProductResult>;

public record DeleteProductResult(bool IsSuccess);

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product Id is required");
    }
}

public class DeleteProductHandler(CatalogDbContext dbContext) : ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
    public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        // Delete Product entity from command object
        // Save to database
        // Return result

        var product = await dbContext.Products.FindAsync([command.ProductId], cancellationToken: cancellationToken);

        if (product is null)
        {
            //throw new Exception($"Product not found: {command.ProductId}");
            throw new ProductNotFoundException(command.ProductId);
        }

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteProductResult(true);
    }
}