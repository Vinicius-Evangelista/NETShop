namespace Basket.Api.Basket.DeleteBasket;

// public record DeleteBasketRequest(string UserName)
//  : ICommand<DeleteBasketResult>;

public record DeleteBasketResponse(bool Success);

public class DeleteBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapDelete(
                pattern: "/basket/{userName}",
                handler: async (string userName, ISender sender) =>
                {
                    var command = new DeleteBasketCommand(
                        UserName: userName
                    );

                    var result = await sender.Send(request: command);

                    var response = new DeleteBasketResponse(
                        Success: result.Success
                    );

                    return Results.Ok(value: response);
                }
            )
            .WithName(endpointName: "Delete Basket")
            .Produces<DeleteBasketResponse>(
                statusCode: StatusCodes.Status200OK
            )
            .ProducesProblem(
                statusCode: StatusCodes.Status500InternalServerError
            )
            .ProducesValidationProblem(
                statusCode: StatusCodes.Status422UnprocessableEntity
            )
            .WithSummary(summary: "Delete Basket")
            .WithDescription(description: "Delete a shopping cart.");
}
