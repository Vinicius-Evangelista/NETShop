namespace Basket.Api.Basket.DeleteBasket;

public record DeleteBasketCommand(string UserName)
    : ICommand<DeleteBasketResult>;

public record DeleteBasketResult(bool Success);

public class DeleteBasketCommandValidator
    : AbstractValidator<DeleteBasketCommand>
{
    public DeleteBasketCommandValidator() =>
        RuleFor(expression: command => command.UserName)
            .NotEmpty()
            .WithMessage(errorMessage: "UserName cannot be empty.");
}

public class DeleteBasketHandler(IBasketRepository repository)
    : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
    public async Task<DeleteBasketResult> Handle(
        DeleteBasketCommand command,
        CancellationToken cancellationToken
    )
    {
        var success = await repository.DeleteBasketAsync(
            userName: command.UserName,
            cancellationToken: cancellationToken
        );

        return new DeleteBasketResult(Success: success);
    }
}
