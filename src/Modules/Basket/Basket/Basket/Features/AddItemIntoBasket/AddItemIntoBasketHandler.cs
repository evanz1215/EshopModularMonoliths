using Catalog.Contracts.Products.Features.GetProductById;

namespace Basket.Basket.Features.AddItemIntoBasket;

public record AddItemIntoBasketCommand(string UserName, ShoppingCartItemDto shoppingCartItem) : ICommand<AddItemIntoBasketResult>;

public record AddItemIntoBasketResult(Guid Id);

public class AddItemIntoBasketCommandValidator : AbstractValidator<AddItemIntoBasketCommand>
{
    public AddItemIntoBasketCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
        RuleFor(x => x.shoppingCartItem.ProductId).NotEmpty().WithMessage("ProductId is required");
        RuleFor(x => x.shoppingCartItem.Quantity).GreaterThan(0).WithMessage("Quantity should be greater than 0");
    }
}

public class AddItemIntoBasketHandler(IBasketRepository repository, ISender sender) : ICommandHandler<AddItemIntoBasketCommand, AddItemIntoBasketResult>
{
    public async Task<AddItemIntoBasketResult> Handle(AddItemIntoBasketCommand command, CancellationToken cancellationToken)
    {
        var shoppingCart = await repository.GetBasket(command.UserName, false, cancellationToken);

        var result = await sender.Send(new GetProductByIdQuery(command.shoppingCartItem.ProductId));

        shoppingCart.AddItem(
            command.shoppingCartItem.ProductId,
            command.shoppingCartItem.Quantity,
            command.shoppingCartItem.Color,
            result.Product.Price,
            result.Product.Name
            );

        await repository.SaveChangesAsync(command.UserName, cancellationToken);

        return new AddItemIntoBasketResult(shoppingCart.Id);
    }
}