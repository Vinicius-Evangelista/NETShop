namespace Basket.Api.Basket.StoreBasket;

public record StoreBasketCommand(ShoppingCart Cart)
    : ICommand<StoreBasketResult>;

public record StoreBasketResult(string UserName);

public class StoreBasketCommandValidator
    : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
        RuleFor(expression: command => command.Cart)
            .NotNull()
            .WithMessage(errorMessage: "Cart cannot be null.");

        RuleFor(expression: command => command.Cart.UserName)
            .NotEmpty()
            .WithMessage(
                errorMessage: "Cart must contain at least one item."
            );
    }
}

public class StoreBasketEndpointHandler(IBasketRepository repository)
    : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(
        StoreBasketCommand command,
        CancellationToken cancellationToken
    )
    {
        await repository.StoreBasketAsync(
            basket: command.Cart,
            cancellationToken: cancellationToken
        );
        return new StoreBasketResult(UserName: command.Cart.UserName);
    }
}
