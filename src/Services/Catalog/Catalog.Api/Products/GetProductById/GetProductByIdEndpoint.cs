namespace Catalog.Api.Products.GetProductById;

public record GetProductRequest();

public record GetProductResponse(Product Product)
{
    public static GetProductResponse ToResponse(
        GetProductResult result
    ) => new(Product: result.Product);
}

public class GetProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet(
                pattern: "/products/{id:guid}",
                handler: async (Guid id, ISender sender) =>
                {
                    var result = await sender.Send(
                        request: new GetProductQuery(Id: id)
                    );

                    var response = GetProductResponse.ToResponse(
                        result: result
                    );

                    return Results.Ok(value: response);
                }
            )
            .WithName(endpointName: "GetProductById")
            .Produces<GetProductResponse>(
                statusCode: StatusCodes.Status200OK
            )
            .WithSummary(summary: "Get a Product by its id")
            .WithDescription(description: "Get a Product by its id");
}
