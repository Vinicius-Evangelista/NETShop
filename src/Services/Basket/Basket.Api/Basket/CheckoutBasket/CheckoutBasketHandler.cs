using Basket.API.Dtos;
using BuildingBlocks.Messaging.Events;
using Mapster;
using MassTransit;
using System.Diagnostics;

namespace Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketCommand(
    BasketCheckoutDto BasketCheckoutDto)
    : ICommand<CheckoutBasketResult>;

public record CheckoutBasketResult(bool IsSuccess);

public class CheckoutBasketCommandValidator
    : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(x => x.BasketCheckoutDto).NotNull()
            .WithMessage("BasketCheckoutDto can't be null");
        RuleFor(x => x.BasketCheckoutDto.UserName).NotEmpty()
            .WithMessage("UserName is required");
    }
}

public class CheckoutBasketCommandHandler(
    IBasketRepository repository,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    private static readonly ActivitySource ActivitySource = new("Basket.CheckoutBasket");

    public async Task<CheckoutBasketResult> Handle(
        CheckoutBasketCommand command,
        CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("CheckoutBasket");
        activity?.SetTag("basket.userName", command.BasketCheckoutDto.UserName);
        activity?.AddEvent(new ActivityEvent("CheckoutBasketStarted"));

        var basket = await repository.GetBasketAsync(
            command.BasketCheckoutDto.UserName, cancellationToken);
        if (basket == null)
        {
            activity?.AddEvent(new ActivityEvent("BasketNotFound"));
            return new CheckoutBasketResult(false);
        }

        var eventMessage =
            command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();
        eventMessage.TotalPrice = basket.TotalPrice;

        await publishEndpoint.Publish(eventMessage,
            cancellationToken);
        activity?.AddEvent(new ActivityEvent("BasketPublished"));

        await repository.DeleteBasketAsync(
            command.BasketCheckoutDto.UserName, cancellationToken);
        activity?.AddEvent(new ActivityEvent("BasketDeleted"));

        return new CheckoutBasketResult(true);
    }
}
