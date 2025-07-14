namespace Basket.Api.Basket.GetBasket;

public record GetBasketResponse(ShoppingCart Cart);

public class GetBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet(
                pattern: "/basket/{userName}",
                handler: async (string userName, ISender sender) =>
                {
                    var result = await sender.Send(
                        request: new GetBasketQuery(
                            UserName: userName
                        )
                    );

                    var response = GetBasketResult.ToResponse(
                        result: result
                    );
                    return response;
                }
            )
            .WithName(endpointName: "Create Basket")
            .Produces<GetBasketResponse>(
                statusCode: StatusCodes.Status201Created
            )
            .ProducesProblem(
                statusCode: StatusCodes.Status500InternalServerError
            )
            .ProducesValidationProblem(
                statusCode: StatusCodes.Status422UnprocessableEntity
            )
            .WithSummary(summary: "Create Product")
            .WithDescription(description: "Create a product.");
}
