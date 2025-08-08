using System.Diagnostics;

namespace Basket.Api.Basket.StoreBasket;

public record StoreBasketRequest(ShoppingCart Cart)
    : ICommand<StoreBasketResponse>
{
    public static StoreBasketCommand FromRequest(
        StoreBasketRequest req
    ) => new(Cart: req.Cart);
}

public record StoreBasketResponse(string UserName)
{
    public static StoreBasketResponse ToResponse(
        StoreBasketResult result
    ) => new(UserName: result.UserName);
}

public class StoreBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost(
                pattern: "/basket",
                handler: async (
                    StoreBasketRequest req,
                    ISender sender
                ) =>
                {

                    var command = StoreBasketRequest.FromRequest(
                        req: req
                    );

                    var result = await sender.Send(request: command);

                    var response = StoreBasketResponse.ToResponse(
                        result: result
                    );

                    return Results.Created(
                        uri: $"/basket/{response.UserName}",
                        value: response
                    );
                }
            )
            .WithName(endpointName: "Store Basket")
            .Produces<StoreBasketResponse>(
                statusCode: StatusCodes.Status201Created
            )
            .ProducesProblem(
                statusCode: StatusCodes.Status500InternalServerError
            )
            .ProducesValidationProblem(
                statusCode: StatusCodes.Status422UnprocessableEntity
            )
            .WithSummary(summary: "Store Basket")
            .WithDescription(description: "Store a shopping cart.");
}
