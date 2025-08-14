using Basket.API.Dtos;
using BuildingBlocks.Messaging.Events;
using Mapster;
using MassTransit;
using System.Diagnostics;
using System.Diagnostics.Metrics;

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
    IPublishEndpoint publishEndpoint,
    ILogger<CheckoutBasketCommandHandler> logger)
    : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    private static readonly ActivitySource ActivitySource =
        new("Basket.CheckoutBasket");

    private static readonly Meter Meter =
        new("Basket.CheckoutBasket.Metrics");

    private static readonly Counter<long> CheckoutCounter =
        Meter.CreateCounter<long>("basket_checkout_count");

    public async Task<CheckoutBasketResult> Handle(
        CheckoutBasketCommand command,
        CancellationToken cancellationToken)
    {
        Log.StartingCheckoutBasket(logger, command.BasketCheckoutDto.UserName);
        using var activity =
            ActivitySource.StartActivity("CheckoutBasket");
        activity?.SetTag("basket.userName",
            command.BasketCheckoutDto.UserName);
        activity?.AddEvent(
            new ActivityEvent("CheckoutBasketStarted"));

        CheckoutCounter.Add(1,
            new KeyValuePair<string, object?>("userName",
                command.BasketCheckoutDto.UserName));

        var basket = await repository.GetBasketAsync(
            command.BasketCheckoutDto.UserName, cancellationToken);
        if (basket == null)
        {
            Log.BasketNotFound(logger, command.BasketCheckoutDto.UserName);
            activity?.AddEvent(new ActivityEvent("BasketNotFound"));
            return new CheckoutBasketResult(false);
        }

        var eventMessage =
            command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();
        eventMessage.TotalPrice = basket.TotalPrice;

        await publishEndpoint.Publish(eventMessage,
            cancellationToken);
        activity?.AddEvent(new ActivityEvent("BasketPublished"));
        Log.BasketPublished(logger, command.BasketCheckoutDto.UserName);

        await repository.DeleteBasketAsync(
            command.BasketCheckoutDto.UserName, cancellationToken);
        activity?.AddEvent(new ActivityEvent("BasketDeleted"));
        Log.BasketDeleted(logger, command.BasketCheckoutDto.UserName);

        return new CheckoutBasketResult(true);
    }
}

public static partial class Log
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Starting CheckoutBasket for user {UserName}")]
    public static partial void StartingCheckoutBasket(ILogger logger, string userName);

    [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "Basket not found for user {UserName}")]
    public static partial void BasketNotFound(ILogger logger, string userName);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Basket published for user {UserName}")]
    public static partial void BasketPublished(ILogger logger, string userName);

    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Basket deleted for user {UserName}")]
    public static partial void BasketDeleted(ILogger logger, string userName);
}
